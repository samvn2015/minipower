namespace Hrm.Domain.Leave.Repositories;

public sealed record LeaveRequestCreateModel(
    Guid EmployeeId,
    string LeaveTypeCode,
    DateOnly FromDate,
    DateOnly ToDate,
    LeaveDayPart DayPart,
    decimal TotalDays,
    string Reason,
    Guid HandoverEmployeeId,
    bool IsEmergency);

public sealed record LeaveRequestSnapshot(
    Guid Id,
    Guid EmployeeId,
    string LeaveTypeCode,
    string? LeaveTypeName,
    DateOnly FromDate,
    DateOnly ToDate,
    LeaveDayPart DayPart,
    decimal TotalDays,
    string Reason,
    Guid HandoverEmployeeId,
    LeaveRequestStatus Status,
    bool IsEmergency,
    DateTime SubmittedAtUtc);

public interface ILeaveRequestRepository
{
    Task<Guid> CreateAsync(LeaveRequestCreateModel model, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<LeaveRequestSnapshot>> ListByEmployeeIdAsync(
        Guid employeeId,
        CancellationToken cancellationToken = default);
}
