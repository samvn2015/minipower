using Hrm.Domain.Leave.Entities;
using Hrm.Domain.Leave.Repositories;
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
        x.SubmittedAtUtc);
}
