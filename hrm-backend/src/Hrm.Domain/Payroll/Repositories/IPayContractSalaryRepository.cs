namespace Hrm.Domain.Payroll.Repositories;

public sealed record PayContractSalarySnapshot(
    Guid EmployeeId,
    string EmployeeCode,
    decimal Amount,
    int DependentCount);

public interface IPayContractSalaryRepository
{
    Task<decimal> GetAmountAsync(Guid employeeId, CancellationToken cancellationToken = default);

    Task<PayContractSalarySnapshot?> FindAsync(
        Guid employeeId,
        CancellationToken cancellationToken = default);

    Task UpsertAsync(
        Guid employeeId,
        string employeeCode,
        decimal amount,
        int dependentCount,
        CancellationToken cancellationToken = default);
}
