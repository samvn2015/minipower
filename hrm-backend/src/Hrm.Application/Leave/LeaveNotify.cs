using Hrm.Domain.Leave;
using Hrm.Domain.Leave.Repositories;

namespace Hrm.Application.Leave;

internal static class LeaveNotify
{
    public static async Task EmitAsync(
        ILeaveNotificationOutbox outbox,
        Guid leaveRequestId,
        Guid employeeId,
        string eventType,
        CancellationToken cancellationToken)
    {
        foreach (var channel in new[] { LeaveNotificationChannels.Email, LeaveNotificationChannels.InApp })
        {
            await outbox.PublishAsync(
                    new LeaveNotificationCreateModel(
                        leaveRequestId,
                        employeeId,
                        eventType,
                        channel,
                        $"{eventType} ({channel})"),
                    cancellationToken)
                .ConfigureAwait(false);
        }
    }
}
