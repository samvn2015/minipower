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
    decimal Ot30,
    decimal ContractAllowance,
    decimal MonthlyAllowance);

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

public sealed record PayAllowanceCatalogDto(string Code, string Name, bool IsActive);

public sealed record PayMonthlyAllowanceDto(
    Guid Id,
    string PeriodYm,
    Guid EmployeeId,
    string EmployeeCode,
    string Code,
    decimal Amount);

public sealed record PayMonthlyAllowanceResult(
    string PeriodYm,
    string EmployeeCode,
    string Code,
    decimal Amount);
