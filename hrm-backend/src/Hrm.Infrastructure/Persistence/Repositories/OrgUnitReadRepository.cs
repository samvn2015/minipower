using Hrm.Domain.Employees.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Hrm.Infrastructure.Persistence.Repositories;

internal sealed class OrgUnitReadRepository(AppDbContext db) : IOrgUnitReadRepository
{
    public async Task<bool> IsActiveAsync(string orgUnitCode, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(orgUnitCode))
            return false;

        var code = orgUnitCode.Trim();
        return await db.OrgUnits
            .AsNoTracking()
            .AnyAsync(
                o => o.Code == code && o.Status == Domain.Employees.OrgUnitStatus.Active,
                cancellationToken);
    }
}
