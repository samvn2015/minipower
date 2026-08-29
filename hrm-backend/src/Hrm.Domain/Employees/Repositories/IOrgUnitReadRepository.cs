namespace Hrm.Domain.Employees.Repositories;

public interface IOrgUnitReadRepository
{
    Task<bool> IsActiveAsync(string orgUnitCode, CancellationToken cancellationToken = default);
}
