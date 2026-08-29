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

    public async Task<IReadOnlyList<LeaveRequestSnapshot>> ListByEmployeeIdAsync(
        Guid employeeId,
        CancellationToken cancellationToken = default) =>
        await db.LeaveRequests.AsNoTracking()
            .Where(x => x.EmployeeId == employeeId)
            .OrderByDescending(x => x.SubmittedAtUtc)
            .Select(x => new LeaveRequestSnapshot(
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
                x.SubmittedAtUtc))
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
}
