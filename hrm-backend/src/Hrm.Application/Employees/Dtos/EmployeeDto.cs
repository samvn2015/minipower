namespace Hrm.Application.Employees.Dtos;

public sealed record EmployeeDto(
    Guid Id,
    string EmployeeCode,
    string? FullName,
    string? Cccd,
    string? EmailCty,
    string? TaxId,
    string? OrgUnitCode,
    EmployeeContractDto? Contract,
    Guid? LineManagerEmployeeId,
    string Status);
