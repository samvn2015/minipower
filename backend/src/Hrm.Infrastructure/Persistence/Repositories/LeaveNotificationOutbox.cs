using Hrm.Domain.Leave;
using Hrm.Domain.Leave.Entities;
using Hrm.Domain.Leave.Repositories;
using Hrm.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Hrm.Infrastructure.Persistence.Repositories;

internal sealed class LeaveNotificationOutbox(AppDbContext db) : ILeaveNotificationOutbox
{
    public async Task PublishAsync(
        LeaveNotificationCreateModel model,
        CancellationToken cancellationToken = default)
    {
        if (LeaveNotificationChannels.IsCrmSales(model.Channel)
            || !LeaveNotificationChannels.IsAllowed(model.Channel))
        {
            throw new InvalidOperationException(
                $"Kênh thông báo cấm hoặc không hỗ trợ: {model.Channel} (LEV-FR-009).");
        }

        db.LeaveNotifications.Add(new LeaveNotification
        {
            Id = Guid.NewGuid(),
            LeaveRequestId = model.LeaveRequestId,
            EmployeeId = model.EmployeeId,
            EventType = model.EventType,
            Channel = model.Channel,
            Message = model.Message,
            CreatedAtUtc = DateTime.UtcNow
        });
        await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<LeaveNotificationSnapshot>> ListByEmployeeAsync(
        Guid employeeId,
        CancellationToken cancellationToken = default) =>
        await db.LeaveNotifications.AsNoTracking()
            .Where(x => x.EmployeeId == employeeId)
            .OrderByDescending(x => x.CreatedAtUtc)
            .Select(x => new LeaveNotificationSnapshot(
                x.Id, x.LeaveRequestId, x.EmployeeId, x.EventType, x.Channel, x.Message, x.CreatedAtUtc))
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
}
