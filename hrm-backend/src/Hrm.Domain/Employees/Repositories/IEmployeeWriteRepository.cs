using Hrm.Domain.Employees;

namespace Hrm.Domain.Employees.Repositories;

public sealed record EmployeeCreateModel(
    string EmployeeCode,
    string? FullName,
    string? Cccd,
    string? EmailCty,
    string? TaxId);

public sealed record EmployeePatch(
    string? FullName,
    string? EmailCty,
    string? Cccd,
    string? TaxId);

public interface IEmployeeWriteRepository
{
    Task<Guid> CreateAsync(EmployeeCreateModel model, CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(Guid id, EmployeePatch patch, CancellationToken cancellationToken = default);
}
