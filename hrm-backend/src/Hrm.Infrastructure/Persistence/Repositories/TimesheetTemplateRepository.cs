using Hrm.Domain.Timekeeping;
using Hrm.Domain.Timekeeping.Entities;
using Hrm.Domain.Timekeeping.Repositories;
using Hrm.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Hrm.Infrastructure.Persistence.Repositories;

internal sealed class TimesheetTemplateRepository(AppDbContext db) : ITimesheetTemplateRepository
{
    public async Task<TimesheetTemplateVersionSnapshot?> FindActiveAsync(
        CancellationToken cancellationToken = default)
    {
        var entity = await db.TimesheetTemplateVersions.AsNoTracking()
            .Include(x => x.Columns)
            .FirstOrDefaultAsync(x => x.Status == TimesheetTemplateStatus.Active, cancellationToken)
            .ConfigureAwait(false);
        return entity is null ? null : Map(entity);
    }

    public async Task<TimesheetTemplateVersionSnapshot?> FindByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var entity = await db.TimesheetTemplateVersions.AsNoTracking()
            .Include(x => x.Columns)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            .ConfigureAwait(false);
        return entity is null ? null : Map(entity);
    }

    public async Task<IReadOnlyList<TimesheetTemplateVersionSnapshot>> ListAsync(
        CancellationToken cancellationToken = default)
    {
        var rows = await db.TimesheetTemplateVersions.AsNoTracking()
            .Include(x => x.Columns)
            .OrderByDescending(x => x.PublishedAtUtc)
            .ThenBy(x => x.VersionCode)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
        return rows.Select(Map).ToList();
    }

    public async Task<Guid> CreateDraftAsync(
        TimesheetTemplateCreateModel model,
        CancellationToken cancellationToken = default)
    {
        var id = Guid.NewGuid();
        var entity = new TimesheetTemplateVersion
        {
            Id = id,
            VersionCode = model.VersionCode.Trim(),
            Name = model.Name.Trim(),
            Status = TimesheetTemplateStatus.Draft,
            Columns = model.Columns.Select(c => new TimesheetTemplateColumn
            {
                Id = Guid.NewGuid(),
                TemplateVersionId = id,
                ColumnKey = c.ColumnKey.Trim(),
                DisplayName = c.DisplayName.Trim(),
                SortOrder = c.SortOrder,
                IsRequired = c.IsRequired,
                MapsTo = c.MapsTo.Trim()
            }).ToList()
        };

        db.TimesheetTemplateVersions.Add(entity);
        await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return id;
    }

    public Task<bool> ExistsByVersionCodeAsync(
        string versionCode,
        CancellationToken cancellationToken = default) =>
        db.TimesheetTemplateVersions.AnyAsync(
            x => x.VersionCode == versionCode,
            cancellationToken);

    public async Task<bool> PublishAsync(
        Guid id,
        string publishedByIdpSubject,
        CancellationToken cancellationToken = default)
    {
        await using var tx = await db.Database.BeginTransactionAsync(cancellationToken).ConfigureAwait(false);

        var target = await db.TimesheetTemplateVersions
            .Include(x => x.Columns)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            .ConfigureAwait(false);
        if (target is null || target.Status != TimesheetTemplateStatus.Draft)
            return false;

        if (target.Columns.Count == 0)
            return false;

        var actives = await db.TimesheetTemplateVersions
            .Where(x => x.Status == TimesheetTemplateStatus.Active)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        foreach (var active in actives)
            active.Status = TimesheetTemplateStatus.Retired;

        target.Status = TimesheetTemplateStatus.Active;
        target.PublishedAtUtc = DateTime.UtcNow;
        target.PublishedByIdpSubject = publishedByIdpSubject;

        await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        await tx.CommitAsync(cancellationToken).ConfigureAwait(false);
        return true;
    }

    public Task<int> CountActiveAsync(CancellationToken cancellationToken = default) =>
        db.TimesheetTemplateVersions.CountAsync(
            x => x.Status == TimesheetTemplateStatus.Active,
            cancellationToken);

    private static TimesheetTemplateVersionSnapshot Map(TimesheetTemplateVersion entity) =>
        new(
            entity.Id,
            entity.VersionCode,
            entity.Name,
            entity.Status,
            entity.PublishedAtUtc,
            entity.PublishedByIdpSubject,
            entity.Columns
                .OrderBy(c => c.SortOrder)
                .Select(c => new TimesheetTemplateColumnSnapshot(
                    c.Id,
                    c.ColumnKey,
                    c.DisplayName,
                    c.SortOrder,
                    c.IsRequired,
                    c.MapsTo))
                .ToList());
}
