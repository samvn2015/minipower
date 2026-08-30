using Hrm.Domain.Employees;

namespace Hrm.Domain.Employees.Repositories;

public sealed record EmployeeSnapshot(
    Guid Id,
    string EmployeeCode,
    string? FullName,
    string? Cccd,
    string? EmailCty,
    string? TaxId,
    string? OrgUnitCode,
    string? EducationLevelCode,
    string? EducationLevelName,
    DateOnly? SeniorityStartDate,
    EmployeeContractSnapshot? Contract,
    Guid? LineManagerEmployeeId,
    EmployeeStatus Status);
