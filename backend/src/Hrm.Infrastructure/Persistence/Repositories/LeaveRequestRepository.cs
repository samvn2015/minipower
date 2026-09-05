using Hrm.Domain.Leave.Entities;
using Hrm.Domain.Leave.Repositories;
using Hrm.Domain.Leave;
using Hrm.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Hrm.Infrastructure.Persistence.Repositories;

internal sealed class LeaveRequestRepository(AppDbContext db) : ILeaveRequestRepository
{
    public async Task<Guid> CreateAsync(
        LeaveRequestCreateModel model,
        CancellationToken cancellationToken = default)
    {
        var entity = new LeaveRequest
        {
            Id = Guid.NewGuid(),
            EmployeeId = model.EmployeeId,
            LeaveTypeCode = model.LeaveTypeCode,
            FromDate = model.FromDate,
            ToDate = model.ToDate,
            DayPart = model.DayPart,
            TotalDays = model.TotalDays,
            Reason = model.Reason,
            HandoverEmployeeId = model.HandoverEmployeeId,
            Status = Domain.Leave.LeaveRequestStatus.PendingC1,
            IsEmergency = model.IsEmergency,
            AttachmentFileName = model.AttachmentFileName,
            AttachmentMatchesCompanyTemplate = model.AttachmentMatchesCompanyTemplate,
            SubmittedAtUtc = DateTime.UtcNow
        };

        db.LeaveRequests.Add(entity);
        await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return entity.Id;
    }

    public async Task<LeaveRequestSnapshot?> FindByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default) =>
        await db.LeaveRequests.AsNoTracking()
            .Where(x => x.Id == id)
            .Select(MapSnapshot)
            .FirstOrDefaultAsync(cancellationToken)
            .ConfigureAwait(false);

    public async Task<IReadOnlyList<LeaveRequestSnapshot>> ListByEmployeeIdAsync(
        Guid employeeId,
        CancellationToken cancellationToken = default) =>
        await db.LeaveRequests.AsNoTracking()
            .Where(x => x.EmployeeId == employeeId)
            .OrderByDescending(x => x.SubmittedAtUtc)
            .Select(MapSnapshot)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

    public async Task<IReadOnlyList<LeaveRequestPendingC1Snapshot>> ListPendingC1ByLineManagerIdAsync(
        Guid lineManagerEmployeeId,
        CancellationToken cancellationToken = default) =>
        await (
            from request in db.LeaveRequests.AsNoTracking()
            join employee in db.Employees.AsNoTracking() on request.EmployeeId equals employee.Id
            where request.Status == Domain.Leave.LeaveRequestStatus.PendingC1
                  && employee.LineManagerEmployeeId == lineManagerEmployeeId
            orderby request.SubmittedAtUtc
            select new LeaveRequestPendingC1Snapshot(
                request.Id,
                request.EmployeeId,
                employee.EmployeeCode,
                employee.FullName,
                request.LeaveTypeCode,
                request.LeaveType != null ? request.LeaveType.Name : null,
                request.FromDate,
                request.ToDate,
                request.DayPart,
                request.TotalDays,
                request.Reason,
                request.HandoverEmployeeId,
                request.IsEmergency,
                request.SubmittedAtUtc))
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

    public async Task<bool> ApproveC1Async(
        Guid id,
        string reviewedByIdpSubject,
        CancellationToken cancellationToken = default)
    {
        var entity = await db.LeaveRequests
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            .ConfigureAwait(false);
        if (entity is null || entity.Status != Domain.Leave.LeaveRequestStatus.PendingC1)
            return false;

        entity.Status = Domain.Leave.LeaveRequestStatus.PendingC2;
        entity.C1ReviewedByIdpSubject = reviewedByIdpSubject;
        entity.C1ReviewedAtUtc = DateTime.UtcNow;
        entity.C1ReviewNote = null;
        await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return true;
    }

    public async Task<bool> RejectC1Async(
        Guid id,
        string reviewedByIdpSubject,
        string? reviewNote,
        CancellationToken cancellationToken = default)
    {
        var entity = await db.LeaveRequests
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            .ConfigureAwait(false);
        if (entity is null || entity.Status != Domain.Leave.LeaveRequestStatus.PendingC1)
            return false;

        entity.Status = Domain.Leave.LeaveRequestStatus.Rejected;
        entity.C1ReviewedByIdpSubject = reviewedByIdpSubject;
        entity.C1ReviewedAtUtc = DateTime.UtcNow;
        entity.C1ReviewNote = reviewNote;
        await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return true;
    }

    public async Task<IReadOnlyList<LeaveRequestPendingC1Snapshot>> ListPendingC2Async(
        CancellationToken cancellationToken = default) =>
        await (
            from request in db.LeaveRequests.AsNoTracking()
            join employee in db.Employees.AsNoTracking() on request.EmployeeId equals employee.Id
            where request.Status == Domain.Leave.LeaveRequestStatus.PendingC2
            orderby request.SubmittedAtUtc
            select new LeaveRequestPendingC1Snapshot(
                request.Id,
                request.EmployeeId,
                employee.EmployeeCode,
                employee.FullName,
                request.LeaveTypeCode,
                request.LeaveType != null ? request.LeaveType.Name : null,
                request.FromDate,
                request.ToDate,
                request.DayPart,
                request.TotalDays,
                request.Reason,
                request.HandoverEmployeeId,
                request.IsEmergency,
                request.SubmittedAtUtc))
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

    public async Task<bool> ApproveC2Async(
        Guid id,
        string reviewedByIdpSubject,
        bool deductsAnnualBalance,
        CancellationToken cancellationToken = default)
    {
        await using var transaction = await db.Database
            .BeginTransactionAsync(cancellationToken)
            .ConfigureAwait(false);

        var entity = await db.LeaveRequests
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            .ConfigureAwait(false);
        if (entity is null || entity.Status != Domain.Leave.LeaveRequestStatus.PendingC2)
            return false;

        if (deductsAnnualBalance)
        {
            var balance = await db.LeaveBalances
                .FirstOrDefaultAsync(
                    x => x.EmployeeId == entity.EmployeeId && x.Year == entity.FromDate.Year,
                    cancellationToken)
                .ConfigureAwait(false);
            if (balance is null || balance.EntitledDays - balance.UsedDays < entity.TotalDays)
                return false;

            balance.UsedDays += entity.TotalDays;
        }

        entity.Status = Domain.Leave.LeaveRequestStatus.Approved;
        entity.C2ReviewedByIdpSubject = reviewedByIdpSubject;
        entity.C2ReviewedAtUtc = DateTime.UtcNow;
        entity.C2ReviewNote = null;

        await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        await transaction.CommitAsync(cancellationToken).ConfigureAwait(false);
        return true;
    }

    public async Task<bool> RejectC2Async(
        Guid id,
        string reviewedByIdpSubject,
        string? reviewNote,
        CancellationToken cancellationToken = default)
    {
        var entity = await db.LeaveRequests
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            .ConfigureAwait(false);
        if (entity is null || entity.Status != Domain.Leave.LeaveRequestStatus.PendingC2)
            return false;

        entity.Status = Domain.Leave.LeaveRequestStatus.Rejected;
        entity.C2ReviewedByIdpSubject = reviewedByIdpSubject;
        entity.C2ReviewedAtUtc = DateTime.UtcNow;
        entity.C2ReviewNote = reviewNote;
        await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return true;
    }

    public async Task<bool> HasOpenOverlapAsync(
        Guid employeeId,
        DateOnly fromDate,
        DateOnly toDate,
        LeaveDayPart dayPart,
        CancellationToken cancellationToken = default)
    {
        var open = await db.LeaveRequests.AsNoTracking()
            .Where(x => x.EmployeeId == employeeId)
            .Where(x => x.Status == Domain.Leave.LeaveRequestStatus.PendingC1
                        || x.Status == Domain.Leave.LeaveRequestStatus.PendingC2
                        || x.Status == Domain.Leave.LeaveRequestStatus.Approved)
            .Select(x => new { x.FromDate, x.ToDate, x.DayPart })
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        return open.Any(x => LeaveOverlapChecker.Overlaps(
            fromDate,
            toDate,
            dayPart,
            x.FromDate,
            x.ToDate,
            x.DayPart));
    }

    public async Task<bool> CancelByEmployeeAsync(
        Guid id,
        Guid employeeId,
        CancellationToken cancellationToken = default)
    {
        var entity = await db.LeaveRequests
            .FirstOrDefaultAsync(x => x.Id == id && x.EmployeeId == employeeId, cancellationToken)
            .ConfigureAwait(false);
        if (entity is null)
            return false;

        if (entity.Status is not (
            Domain.Leave.LeaveRequestStatus.PendingC1
            or Domain.Leave.LeaveRequestStatus.PendingC2))
        {
            return false;
        }

        entity.Status = Domain.Leave.LeaveRequestStatus.Cancelled;
        await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return true;
    }

    public async Task<IReadOnlyList<ApprovedLeaveForTimesheetSnapshot>> ListApprovedOverlappingPeriodAsync(
        string periodYm,
        IReadOnlyList<Guid> employeeIds,
        CancellationToken cancellationToken = default)
    {
        if (employeeIds.Count == 0
            || periodYm.Length != 7
            || periodYm[4] != '-'
            || !int.TryParse(periodYm.AsSpan(0, 4), out var year)
            || !int.TryParse(periodYm.AsSpan(5, 2), out var month)
            || month is < 1 or > 12)
        {
            return [];
        }

        var monthStart = new DateOnly(year, month, 1);
        var monthEnd = monthStart.AddMonths(1).AddDays(-1);
        var idSet = employeeIds.ToHashSet();

        return await db.LeaveRequests.AsNoTracking()
            .Include(x => x.LeaveType)
            .Where(x => x.Status == Domain.Leave.LeaveRequestStatus.Approved
                        && idSet.Contains(x.EmployeeId)
                        && x.FromDate <= monthEnd
                        && x.ToDate >= monthStart)
            .Select(x => new ApprovedLeaveForTimesheetSnapshot(
                x.Id,
                x.EmployeeId,
                x.LeaveTypeCode,
                x.LeaveType != null && x.LeaveType.DeductsAnnualBalance,
                x.FromDate,
                x.ToDate,
                x.TotalDays))
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    private static readonly System.Linq.Expressions.Expression<
        Func<LeaveRequest, LeaveRequestSnapshot>> MapSnapshot = x => new LeaveRequestSnapshot(
        x.Id,
        x.EmployeeId,
        x.LeaveTypeCode,
        x.LeaveType != null ? x.LeaveType.Name : null,
        x.FromDate,
        x.ToDate,
        x.DayPart,
        x.TotalDays,
        x.Reason,
        x.HandoverEmployeeId,
        x.Status,
        x.IsEmergency,
        x.SubmittedAtUtc,
        x.AttachmentFileName,
        x.AttachmentMatchesCompanyTemplate);
}
