using Hrm.Domain.Employees;

namespace Hrm.Application.Employees.Dtos;

public sealed record EmployeeDto(
    Guid Id,
    string EmployeeCode,
    string? FullName,
    string? Cccd,
    string? EmailCty,
    string? TaxId,
    Guid? LineManagerEmployeeId,
    string Status);
