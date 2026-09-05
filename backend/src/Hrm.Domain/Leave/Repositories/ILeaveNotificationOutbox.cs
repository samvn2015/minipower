namespace Hrm.Domain.Leave.Repositories;

public sealed record LeaveNotificationCreateModel(
    Guid LeaveRequestId,
    Guid EmployeeId,
    string EventType,
    string Channel,
    string Message);

public sealed record LeaveNotificationSnapshot(
    Guid Id,
    Guid LeaveRequestId,
    Guid EmployeeId,
    string EventType,
    string Channel,
    string Message,
    DateTime CreatedAtUtc);

public interface ILeaveNotificationOutbox
{
    Task PublishAsync(LeaveNotificationCreateModel model, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<LeaveNotificationSnapshot>> ListByEmployeeAsync(
        Guid employeeId,
        CancellationToken cancellationToken = default);
}
