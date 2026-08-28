namespace Hrm.Domain.Employees.Repositories;

public interface IEmployeeReadRepository
{
    Task<IReadOnlyList<EmployeeSnapshot>> ListAsync(CancellationToken cancellationToken = default);

    Task<EmployeeSnapshot?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<EmployeeSnapshot?> FindByEmployeeCodeAsync(
        string employeeCode,
        CancellationToken cancellationToken = default);

    Task<EmployeeSnapshot?> FindByEmailCtyAsync(
        string emailCty,
        CancellationToken cancellationToken = default);

    Task<EmployeeUniqueField?> FindDuplicateAsync(
        string employeeCode,
        string? cccd,
        string? emailCty,
        string? taxId,
        Guid? excludeEmployeeId = null,
        CancellationToken cancellationToken = default);
}
