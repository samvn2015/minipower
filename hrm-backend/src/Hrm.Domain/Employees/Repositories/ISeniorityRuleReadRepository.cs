using Hrm.Domain.Employees;

namespace Hrm.Domain.Employees.Repositories;

public sealed record SeniorityRuleSnapshot(string Code, SeniorityBasisType BasisType);

public interface ISeniorityRuleReadRepository
{
    Task<SeniorityRuleSnapshot?> GetActiveAsync(CancellationToken cancellationToken = default);
}
