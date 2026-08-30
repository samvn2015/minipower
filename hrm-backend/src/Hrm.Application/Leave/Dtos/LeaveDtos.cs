namespace Hrm.Application.Leave.Dtos;

public sealed record LeaveTypeDto(
    string Code,
    string Name,
    bool DeductsAnnualBalance);

public sealed record LeaveBalanceDto(
    int Year,
    decimal EntitledDays,
    decimal UsedDays,
    decimal RemainingDays);

public sealed record LeaveRequestDto(
    Guid Id,
    string LeaveTypeCode,
    string? LeaveTypeName,
    string FromDate,
    string ToDate,
    string DayPart,
    decimal TotalDays,
    string Reason,
    Guid HandoverEmployeeId,
    string Status,
    bool IsEmergency);

public sealed record LeaveRequestCreateResult(Guid Id, string Status, decimal TotalDays);

public sealed record LeaveRequestActionResult(Guid Id, string Status);

public sealed record LeaveRequestPendingC1Dto(
    Guid Id,
    string EmployeeCode,
    string? EmployeeFullName,
    string LeaveTypeCode,
    string? LeaveTypeName,
    string FromDate,
    string ToDate,
    string DayPart,
    decimal TotalDays,
    string Reason,
    Guid HandoverEmployeeId,
    bool IsEmergency,
    string SubmittedAtUtc);
