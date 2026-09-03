namespace Hrm.Application.Payroll.Dtos;

public sealed record PayLineDto(
    Guid Id,
    Guid EmployeeId,
    string EmployeeCode,
    decimal WorkDays,
    decimal LeaveDaysUnpaid,
    decimal LeaveDaysPaid,
    decimal NTinh,
    decimal TimeWageFactor,
    decimal Ot15,
    decimal Ot20,
    decimal Ot30);

public sealed record PayPeriodDto(
    Guid Id,
    string PeriodYm,
    string Status,
    int LineCount,
    decimal StandardWorkDays,
    bool HasNTinhOverCap,
    IReadOnlyList<string> OverCapEmployeeCodes,
    IReadOnlyList<PayLineDto> Lines);

public sealed record PayRunResult(
    Guid PeriodId,
    string PeriodYm,
    string Status,
    int LineCount);

public sealed record PayWorkdayCalendarResult(string PeriodYm, decimal StandardWorkDays);
