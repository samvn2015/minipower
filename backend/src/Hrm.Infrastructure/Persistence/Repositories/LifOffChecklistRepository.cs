using Hrm.Domain.Lifecycle.Entities;
using Hrm.Domain.Lifecycle.Repositories;
using Hrm.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Hrm.Infrastructure.Persistence.Repositories;

internal sealed class LifOffChecklistRepository(AppDbContext db) : ILifOffChecklistRepository
{
    public async Task<IReadOnlyList<LifOffChecklistItemSnapshot>> ListActiveItemsAsync(
        CancellationToken cancellationToken = default)
    {
        var rows = await db.LifOffChecklistItems.AsNoTracking()
            .Where(x => x.IsActive)
            .OrderBy(x => x.SortOrder)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
        return rows.Select(x => new LifOffChecklistItemSnapshot(x.Code, x.Name, x.IsMust, x.SortOrder))
            .ToList();
    }

    public async Task<IReadOnlyList<LifOffChecklistTickSnapshot>> ListTicksAsync(
        Guid caseId,
        CancellationToken cancellationToken = default)
    {
        var rows = await db.LifOffChecklistTicks.AsNoTracking()
            .Where(x => x.OffboardingCaseId == caseId)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
        return rows.Select(x => new LifOffChecklistTickSnapshot(
            x.ItemCode, x.IsChecked, x.CheckedByIdpSubject, x.CheckedAtUtc)).ToList();
    }

    public async Task UpsertTickAsync(
        Guid caseId,
        string itemCode,
        bool isChecked,
        string actorIdpSubject,
        CancellationToken cancellationToken = default)
    {
        var row = await db.LifOffChecklistTicks
            .FirstOrDefaultAsync(
                x => x.OffboardingCaseId == caseId && x.ItemCode.ToLower() == itemCode.ToLower(),
                cancellationToken)
            .ConfigureAwait(false);

        if (row is null)
        {
            row = new LifOffChecklistTick
            {
                Id = Guid.NewGuid(),
                OffboardingCaseId = caseId,
                ItemCode = itemCode
            };
            db.LifOffChecklistTicks.Add(row);
        }

        row.IsChecked = isChecked;
        row.CheckedByIdpSubject = isChecked ? actorIdpSubject : null;
        row.CheckedAtUtc = isChecked ? DateTime.UtcNow : null;

        await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task<bool> AllMustCheckedAsync(Guid caseId, CancellationToken cancellationToken = default)
    {
        var mustCodes = await db.LifOffChecklistItems.AsNoTracking()
            .Where(x => x.IsActive && x.IsMust)
            .Select(x => x.Code)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        if (mustCodes.Count == 0)
            return false;

        var ticks = await db.LifOffChecklistTicks.AsNoTracking()
            .Where(x => x.OffboardingCaseId == caseId && x.IsChecked)
            .Select(x => x.ItemCode)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        var set = new HashSet<string>(ticks, StringComparer.OrdinalIgnoreCase);
        return mustCodes.All(c => set.Contains(c));
    }
}
