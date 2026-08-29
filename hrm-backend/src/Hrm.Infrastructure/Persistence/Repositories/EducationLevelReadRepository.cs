using Hrm.Domain.Employees;
using Hrm.Domain.Employees.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Hrm.Infrastructure.Persistence.Repositories;

internal sealed class EducationLevelReadRepository(AppDbContext db) : IEducationLevelReadRepository
{
    public async Task<bool> IsActiveAsync(string code, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(code))
            return false;

        var normalized = code.Trim();
        return await db.EducationLevels
            .AsNoTracking()
            .AnyAsync(
                x => x.Code == normalized && x.Status == EducationLevelStatus.Active,
                cancellationToken);
    }

    public async Task<IReadOnlyList<EducationLevelSnapshot>> ListActiveAsync(
        CancellationToken cancellationToken = default) =>
        await db.EducationLevels
            .AsNoTracking()
            .Where(x => x.Status == EducationLevelStatus.Active)
            .OrderBy(x => x.Code)
            .Select(x => new EducationLevelSnapshot(x.Code, x.Name))
            .ToArrayAsync(cancellationToken);
}
