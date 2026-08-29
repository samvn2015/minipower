namespace Hrm.Application.Employees.Dtos;

public sealed record EmployeeDto(
    Guid Id,
    string EmployeeCode,
    string? FullName,
    string? Cccd,
    string? EmailCty,
    string? TaxId,
    string? OrgUnitCode,
    string? EducationLevelCode,
    string? EducationLevelName,
    SeniorityDto? Seniority,
    EmployeeContractDto? Contract,
    Guid? LineManagerEmployeeId,
    string Status);
