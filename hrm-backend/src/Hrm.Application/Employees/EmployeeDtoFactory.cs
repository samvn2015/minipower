using Hrm.Application.Employees.Dtos;
using Hrm.Domain.Employees.Repositories;

namespace Hrm.Application.Employees;

public sealed class EmployeeDtoFactory(ISeniorityRuleReadRepository seniorityRules)
{
    public async Task<EmployeeDto> MapAsync(
        EmployeeSnapshot snapshot,
        CancellationToken cancellationToken = default)
    {
        var rule = await seniorityRules.GetActiveAsync(cancellationToken).ConfigureAwait(false);
        var seniority = SeniorityCalculator.Calculate(
            snapshot,
            rule,
            DateOnly.FromDateTime(DateTime.UtcNow));
        return EmployeeDtoMapper.Map(snapshot, seniority);
    }
}
