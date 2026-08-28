namespace Hrm.Domain.Employees.Repositories;

public sealed record EmployeePatch(
    string? FullName,
    string? EmailCty,
    string? Cccd);

public interface IEmployeeWriteRepository
{
    Task<bool> UpdateAsync(Guid id, EmployeePatch patch, CancellationToken cancellationToken = default);
}
