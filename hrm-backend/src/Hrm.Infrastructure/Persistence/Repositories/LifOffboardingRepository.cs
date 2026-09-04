using Hrm.Domain.Lifecycle;
using Hrm.Domain.Lifecycle.Entities;
using Hrm.Domain.Lifecycle.Repositories;
using Hrm.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Hrm.Infrastructure.Persistence.Repositories;

internal sealed class LifOffboardingRepository(AppDbContext db) : ILifOffboardingRepository
{
    public async Task<LifOffboardingSnapshot> CreateAsync(
        LifOffboardingCreateModel model,
        CancellationToken cancellationToken = default)
    {
        var row = new LifOffboardingCase
        {
            Id = Guid.NewGuid(),
            EmployeeId = model.EmployeeId,
            EmployeeCode = model.EmployeeCode,
            Source = model.Source,
            Status = LifOffboardingStatus.Open,
            LastWorkingDayN = null,
            ResignationSignedDate = model.ResignationSignedDate,
            CreatedAtUtc = DateTime.UtcNow,
            CreatedByIdpSubject = model.CreatedByIdpSubject,
            Note = model.Note
        };
        db.LifOffboardingCases.Add(row);
        await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Map(row);
    }

    public async Task<IReadOnlyList<LifOffboardingSnapshot>> ListOpenAsync(
        CancellationToken cancellationToken = default)
    {
        var rows = await db.LifOffboardingCases.AsNoTracking()
            .Where(x => x.Status == LifOffboardingStatus.Open || x.Status == LifOffboardingStatus.ConfirmedN)
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
        return rows.Select(Map).ToList();
    }

    public async Task<IReadOnlyList<LifOffboardingSnapshot>> ListAsync(
        CancellationToken cancellationToken = default)
    {
        var rows = await db.LifOffboardingCases.AsNoTracking()
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
        return rows.Select(Map).ToList();
    }

    public async Task<LifOffboardingSnapshot?> FindByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var row = await db.LifOffboardingCases.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            .ConfigureAwait(false);
        return row is null ? null : Map(row);
    }

    public async Task<LifOffboardingSnapshot> ConfirmNAsync(
        Guid id,
        DateOnly lastWorkingDayN,
        string confirmedByIdpSubject,
        CancellationToken cancellationToken = default)
    {
        var row = await db.LifOffboardingCases
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            .ConfigureAwait(false)
            ?? throw new InvalidOperationException("Offboarding case not found.");

        row.LastWorkingDayN = lastWorkingDayN;
        row.Status = LifOffboardingStatus.ConfirmedN;
        row.ConfirmedByIdpSubject = confirmedByIdpSubject;
        row.ConfirmedAtUtc = DateTime.UtcNow;

        await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Map(row);
    }

    public async Task<LifOffboardingSnapshot> CloseAsync(
        Guid id,
        string closedByIdpSubject,
        CancellationToken cancellationToken = default)
    {
        var row = await db.LifOffboardingCases
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            .ConfigureAwait(false)
            ?? throw new InvalidOperationException("Offboarding case not found.");

        row.Status = LifOffboardingStatus.Closed;
        row.Note = string.IsNullOrWhiteSpace(row.Note)
            ? $"Closed by {closedByIdpSubject}"
            : $"{row.Note} | Closed by {closedByIdpSubject}";

        await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Map(row);
    }

    private static LifOffboardingSnapshot Map(LifOffboardingCase x) =>
        new(
            x.Id,
            x.EmployeeId,
            x.EmployeeCode,
            x.Source,
            x.Status,
            x.LastWorkingDayN,
            x.ResignationSignedDate,
            x.ConfirmedByIdpSubject,
            x.ConfirmedAtUtc,
            x.CreatedAtUtc,
            x.CreatedByIdpSubject,
            x.Note);
}
