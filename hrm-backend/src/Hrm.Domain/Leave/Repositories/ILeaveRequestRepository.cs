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

public sealed record LeaveRequestPendingC1Snapshot(
    Guid Id,
    Guid EmployeeId,
    string EmployeeCode,
    string? EmployeeFullName,
    string LeaveTypeCode,
    string? LeaveTypeName,
    DateOnly FromDate,
    DateOnly ToDate,
    LeaveDayPart DayPart,
    decimal TotalDays,
    string Reason,
    Guid HandoverEmployeeId,
    bool IsEmergency,
    DateTime SubmittedAtUtc);

public interface ILeaveRequestRepository
{
    Task<Guid> CreateAsync(LeaveRequestCreateModel model, CancellationToken cancellationToken = default);

    Task<LeaveRequestSnapshot?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<LeaveRequestSnapshot>> ListByEmployeeIdAsync(
        Guid employeeId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<LeaveRequestPendingC1Snapshot>> ListPendingC1ByLineManagerIdAsync(
        Guid lineManagerEmployeeId,
        CancellationToken cancellationToken = default);

    Task<bool> ApproveC1Async(
        Guid id,
        string reviewedByIdpSubject,
        CancellationToken cancellationToken = default);

    Task<bool> RejectC1Async(
        Guid id,
        string reviewedByIdpSubject,
        string? reviewNote,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<LeaveRequestPendingC1Snapshot>> ListPendingC2Async(
        CancellationToken cancellationToken = default);

    Task<bool> ApproveC2Async(
        Guid id,
        string reviewedByIdpSubject,
        bool deductsAnnualBalance,
        CancellationToken cancellationToken = default);

    Task<bool> RejectC2Async(
        Guid id,
        string reviewedByIdpSubject,
        string? reviewNote,
        CancellationToken cancellationToken = default);

    Task<bool> HasOpenOverlapAsync(
        Guid employeeId,
        DateOnly fromDate,
        DateOnly toDate,
        LeaveDayPart dayPart,
        CancellationToken cancellationToken = default);

    Task<bool> CancelByEmployeeAsync(
        Guid id,
        Guid employeeId,
        CancellationToken cancellationToken = default);
}
