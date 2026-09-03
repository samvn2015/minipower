namespace Hrm.Domain.Payroll.Repositories;

public sealed record PayRegulationSnapshot(string Code, string Name, decimal DecimalValue);

public interface IPayRegulationReadRepository
{
    Task<PayRegulationSnapshot?> FindByCodeAsync(string code, CancellationToken cancellationToken = default);
}
