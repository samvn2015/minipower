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
