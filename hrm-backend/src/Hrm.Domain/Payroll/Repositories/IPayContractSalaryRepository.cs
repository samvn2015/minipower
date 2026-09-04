namespace Hrm.Domain.Payroll.Repositories;

public interface IPayContractSalaryRepository
{
    Task<decimal> GetAmountAsync(Guid employeeId, CancellationToken cancellationToken = default);
}
