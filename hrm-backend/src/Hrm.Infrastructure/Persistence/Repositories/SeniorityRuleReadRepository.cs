using Hrm.Domain.Employees;
using Hrm.Domain.Employees.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Hrm.Infrastructure.Persistence.Repositories;

internal sealed class SeniorityRuleReadRepository(AppDbContext db) : ISeniorityRuleReadRepository
{
    public async Task<SeniorityRuleSnapshot?> GetActiveAsync(CancellationToken cancellationToken = default) =>
        await db.SeniorityRules
            .AsNoTracking()
            .Where(x => x.Status == SeniorityRuleStatus.Active)
            .OrderBy(x => x.Code)
            .Select(x => new SeniorityRuleSnapshot(x.Code, x.BasisType))
            .FirstOrDefaultAsync(cancellationToken);
}
