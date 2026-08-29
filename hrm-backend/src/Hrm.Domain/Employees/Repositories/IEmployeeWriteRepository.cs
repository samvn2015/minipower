using Hrm.Domain.Employees;

namespace Hrm.Domain.Employees.Repositories;

public sealed record EmployeeCreateModel(
    string EmployeeCode,
    string? FullName,
    string? Cccd,
    string? EmailCty,
    string? TaxId,
    string? OrgUnitCode,
    string? EducationLevelCode,
    DateOnly? SeniorityStartDate,
    EmployeeContractUpsert? Contract);

public sealed record EmployeeContractUpsert(
    string ContractType,
    DateOnly StartDate,
    DateOnly? EndDate,
    bool IsProbation);

public sealed record EmployeePatch(
    string? FullName,
    string? EmailCty,
    string? Cccd,
    string? TaxId,
    string? OrgUnitCode,
    string? EducationLevelCode,
    DateOnly? SeniorityStartDate,
    EmployeeContractUpsert? Contract);

public interface IEmployeeWriteRepository
{
    Task<Guid> CreateAsync(EmployeeCreateModel model, CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(Guid id, EmployeePatch patch, CancellationToken cancellationToken = default);

    Task SetLineManagerAsync(
        Guid employeeId,
        Guid lineManagerEmployeeId,
        CancellationToken cancellationToken = default);
}
