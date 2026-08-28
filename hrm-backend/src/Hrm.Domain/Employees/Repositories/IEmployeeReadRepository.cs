namespace Hrm.Domain.Employees.Repositories;

public interface IEmployeeReadRepository
{
    Task<EmployeeSnapshot?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<EmployeeSnapshot?> FindByEmployeeCodeAsync(
        string employeeCode,
        CancellationToken cancellationToken = default);
}
