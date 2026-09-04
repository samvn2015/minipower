using Hrm.Domain.Lifecycle;
using Hrm.Domain.Lifecycle.Entities;
using Hrm.Domain.Lifecycle.Repositories;
using Hrm.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Hrm.Infrastructure.Persistence.Repositories;

internal sealed class LifOnboardingRepository(AppDbContext db) : ILifOnboardingRepository
{
    public async Task<LifOnboardingSnapshot> CreateAsync(
        LifOnboardingCreateModel model,
        CancellationToken cancellationToken = default)
    {
        var row = new LifOnboardingCase
        {
            Id = Guid.NewGuid(),
            EmployeeId = model.EmployeeId,
            EmployeeCode = model.EmployeeCode,
            Status = LifOnboardingStatus.Open,
            CreatedAtUtc = DateTime.UtcNow,
            CreatedByIdpSubject = model.CreatedByIdpSubject,
            Note = model.Note
        };
        db.LifOnboardingCases.Add(row);
        await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Map(row);
    }

    public async Task<IReadOnlyList<LifOnboardingSnapshot>> ListAsync(
        CancellationToken cancellationToken = default)
    {
        var rows = await db.LifOnboardingCases.AsNoTracking()
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
        return rows.Select(Map).ToList();
    }

    public async Task<LifOnboardingSnapshot?> FindByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var row = await db.LifOnboardingCases.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            .ConfigureAwait(false);
        return row is null ? null : Map(row);
    }

    public async Task<LifOnboardingSnapshot> MarkProvisionedAsync(
        Guid id,
        string systemCode,
        string actorIdpSubject,
        CancellationToken cancellationToken = default)
    {
        var row = await db.LifOnboardingCases
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            .ConfigureAwait(false)
            ?? throw new InvalidOperationException("Onboarding case not found.");

        var now = DateTime.UtcNow;
        if (string.Equals(systemCode, LifProvisionSystems.EmailCty, StringComparison.OrdinalIgnoreCase))
        {
            row.EmailCtyProvisioned = true;
            row.EmailCtyProvisionedAtUtc = now;
        }
        else if (string.Equals(systemCode, LifProvisionSystems.Git, StringComparison.OrdinalIgnoreCase))
        {
            row.GitProvisioned = true;
            row.GitProvisionedAtUtc = now;
        }
        else if (string.Equals(systemCode, LifProvisionSystems.CrmSp, StringComparison.OrdinalIgnoreCase))
        {
            row.CrmSpProvisioned = true;
            row.CrmSpProvisionedAtUtc = now;
        }
        else if (string.Equals(systemCode, LifProvisionSystems.Chat, StringComparison.OrdinalIgnoreCase))
        {
            row.ChatProvisioned = true;
            row.ChatProvisionedAtUtc = now;
        }
        else
            throw new InvalidOperationException("Unknown provision system.");

        _ = actorIdpSubject;
        await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Map(row);
    }

    public async Task<LifOnboardingSnapshot> CloseAsync(
        Guid id,
        string closedByIdpSubject,
        CancellationToken cancellationToken = default)
    {
        var row = await db.LifOnboardingCases
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            .ConfigureAwait(false)
            ?? throw new InvalidOperationException("Onboarding case not found.");

        row.Status = LifOnboardingStatus.Closed;
        row.ClosedByIdpSubject = closedByIdpSubject;
        row.ClosedAtUtc = DateTime.UtcNow;
        await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Map(row);
    }

    private static LifOnboardingSnapshot Map(LifOnboardingCase x) =>
        new(
            x.Id,
            x.EmployeeId,
            x.EmployeeCode,
            x.Status,
            x.CreatedAtUtc,
            x.CreatedByIdpSubject,
            x.Note,
            x.EmailCtyProvisioned,
            x.GitProvisioned,
            x.CrmSpProvisioned,
            x.ChatProvisioned,
            x.EmailCtyProvisionedAtUtc,
            x.GitProvisionedAtUtc,
            x.CrmSpProvisionedAtUtc,
            x.ChatProvisionedAtUtc,
            x.ClosedByIdpSubject,
            x.ClosedAtUtc);
}

internal sealed class LifOnChecklistRepository(AppDbContext db) : ILifOnChecklistRepository
{
    public async Task<IReadOnlyList<LifOnChecklistItemSnapshot>> ListActiveItemsAsync(
        CancellationToken cancellationToken = default)
    {
        var rows = await db.LifOnChecklistItems.AsNoTracking()
            .Where(x => x.IsActive)
            .OrderBy(x => x.SortOrder)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
        return rows.Select(x => new LifOnChecklistItemSnapshot(x.Code, x.Name, x.IsMust, x.SortOrder)).ToList();
    }

    public async Task<IReadOnlyList<LifOnChecklistTickSnapshot>> ListTicksAsync(
        Guid caseId,
        CancellationToken cancellationToken = default)
    {
        var rows = await db.LifOnChecklistTicks.AsNoTracking()
            .Where(x => x.OnboardingCaseId == caseId)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
        return rows.Select(x => new LifOnChecklistTickSnapshot(
            x.ItemCode, x.IsChecked, x.CheckedByIdpSubject, x.CheckedAtUtc)).ToList();
    }

    public async Task UpsertTickAsync(
        Guid caseId,
        string itemCode,
        bool isChecked,
        string actorIdpSubject,
        CancellationToken cancellationToken = default)
    {
        var row = await db.LifOnChecklistTicks
            .FirstOrDefaultAsync(
                x => x.OnboardingCaseId == caseId && x.ItemCode == itemCode,
                cancellationToken)
            .ConfigureAwait(false);

        if (row is null)
        {
            db.LifOnChecklistTicks.Add(new LifOnChecklistTick
            {
                Id = Guid.NewGuid(),
                OnboardingCaseId = caseId,
                ItemCode = itemCode,
                IsChecked = isChecked,
                CheckedByIdpSubject = isChecked ? actorIdpSubject : null,
                CheckedAtUtc = isChecked ? DateTime.UtcNow : null
            });
        }
        else
        {
            row.IsChecked = isChecked;
            row.CheckedByIdpSubject = isChecked ? actorIdpSubject : null;
            row.CheckedAtUtc = isChecked ? DateTime.UtcNow : null;
        }

        await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task<bool> AllMustCheckedAsync(Guid caseId, CancellationToken cancellationToken = default)
    {
        var must = await db.LifOnChecklistItems.AsNoTracking()
            .Where(x => x.IsActive && x.IsMust)
            .Select(x => x.Code)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
        if (must.Count == 0) return true;

        var checkedCodes = await db.LifOnChecklistTicks.AsNoTracking()
            .Where(x => x.OnboardingCaseId == caseId && x.IsChecked)
            .Select(x => x.ItemCode)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
        var set = new HashSet<string>(checkedCodes, StringComparer.OrdinalIgnoreCase);
        return must.All(set.Contains);
    }
}
