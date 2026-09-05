using Hrm.Domain.Probation;
using Hrm.Domain.Probation.Entities;
using Hrm.Domain.Probation.Repositories;
using Hrm.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Hrm.Infrastructure.Persistence.Repositories;

internal sealed class ProbationMasterReadRepository(AppDbContext db) : IProbationMasterReadRepository
{
    public async Task<IReadOnlyList<ProbationOutcomeSnapshot>> ListOutcomesAsync(
        CancellationToken cancellationToken = default)
    {
        var rows = await db.ProbationOutcomes.AsNoTracking()
            .Where(x => x.IsActive)
            .OrderBy(x => x.SortOrder)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
        return rows.Select(x => new ProbationOutcomeSnapshot(x.Code, x.Name, x.SortOrder)).ToList();
    }

    public Task<bool> OutcomeExistsAsync(string code, CancellationToken cancellationToken = default) =>
        db.ProbationOutcomes.AnyAsync(
            x => x.IsActive && x.Code.ToLower() == code.ToLower(),
            cancellationToken);

    public async Task<IReadOnlyList<ProbationCriterionSnapshot>> ListCriteriaAsync(
        CancellationToken cancellationToken = default)
    {
        var rows = await db.ProbationCriteria.AsNoTracking()
            .Where(x => x.IsActive)
            .OrderBy(x => x.SortOrder)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
        return rows.Select(x => new ProbationCriterionSnapshot(x.Code, x.Name, x.SortOrder)).ToList();
    }

    public Task<bool> CriterionExistsAsync(string code, CancellationToken cancellationToken = default) =>
        db.ProbationCriteria.AnyAsync(
            x => x.IsActive && x.Code.ToLower() == code.ToLower(),
            cancellationToken);

    public async Task<IReadOnlyList<ProbationExtendDurationSnapshot>> ListExtendDurationsAsync(
        CancellationToken cancellationToken = default)
    {
        var rows = await db.ProbationExtendDurations.AsNoTracking()
            .Where(x => x.IsActive)
            .OrderBy(x => x.SortOrder)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
        return rows.Select(x => new ProbationExtendDurationSnapshot(x.Code, x.Name, x.Months, x.SortOrder))
            .ToList();
    }

    public async Task<ProbationExtendDurationSnapshot?> FindExtendDurationAsync(
        string code,
        CancellationToken cancellationToken = default)
    {
        var row = await db.ProbationExtendDurations.AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.IsActive && x.Code.ToLower() == code.ToLower(),
                cancellationToken)
            .ConfigureAwait(false);
        return row is null
            ? null
            : new ProbationExtendDurationSnapshot(row.Code, row.Name, row.Months, row.SortOrder);
    }
}

internal sealed class ProbationEvaluationRepository(AppDbContext db) : IProbationEvaluationRepository
{
    public async Task<ProbationEvaluationSnapshot?> FindOpenByEmployeeAsync(
        Guid employeeId,
        CancellationToken cancellationToken = default)
    {
        var row = await db.ProbationEvaluations.AsNoTracking()
            .Where(x => x.EmployeeId == employeeId && x.Status != ProbationEvaluationStatus.Decided)
            .OrderByDescending(x => x.Id)
            .FirstOrDefaultAsync(cancellationToken)
            .ConfigureAwait(false);
        return row is null ? null : Map(row);
    }

    public async Task<ProbationEvaluationSnapshot?> FindByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var row = await db.ProbationEvaluations.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            .ConfigureAwait(false);
        return row is null ? null : Map(row);
    }

    public async Task<IReadOnlyList<ProbationEvaluationSnapshot>> ListAsync(
        CancellationToken cancellationToken = default)
    {
        var rows = await db.ProbationEvaluations.AsNoTracking()
            .OrderByDescending(x => x.DecidedAtUtc ?? x.ProposedAtUtc ?? DateTime.MinValue)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
        return rows.Select(Map).ToList();
    }

    public async Task<ProbationEvaluationSnapshot> UpsertProposeAsync(
        Guid employeeId,
        string employeeCode,
        DateOnly probationEndDate,
        string outcomeCode,
        string proposedByIdpSubject,
        string? note,
        string? criteriaPayloadJson,
        CancellationToken cancellationToken = default)
    {
        var row = await db.ProbationEvaluations
            .Where(x => x.EmployeeId == employeeId && x.Status != ProbationEvaluationStatus.Decided)
            .OrderByDescending(x => x.Id)
            .FirstOrDefaultAsync(cancellationToken)
            .ConfigureAwait(false);

        if (row is null)
        {
            row = new ProbationEvaluation
            {
                Id = Guid.NewGuid(),
                EmployeeId = employeeId,
                EmployeeCode = employeeCode,
                ProbationEndDate = probationEndDate
            };
            db.ProbationEvaluations.Add(row);
        }

        row.ProbationEndDate = probationEndDate;
        row.Status = ProbationEvaluationStatus.Proposed;
        row.ProposedOutcomeCode = outcomeCode;
        row.ProposedByIdpSubject = proposedByIdpSubject;
        row.ProposedAtUtc = DateTime.UtcNow;
        row.ProposedNote = note;
        row.CriteriaPayloadJson = criteriaPayloadJson;

        await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Map(row);
    }

    public async Task<ProbationEvaluationSnapshot> DecideAsync(
        Guid employeeId,
        string employeeCode,
        DateOnly probationEndDate,
        string outcomeCode,
        string decidedByIdpSubject,
        string? note,
        string? extendDurationCode,
        string? criteriaPayloadJson,
        CancellationToken cancellationToken = default)
    {
        var row = await db.ProbationEvaluations
            .Where(x => x.EmployeeId == employeeId && x.Status != ProbationEvaluationStatus.Decided)
            .OrderByDescending(x => x.Id)
            .FirstOrDefaultAsync(cancellationToken)
            .ConfigureAwait(false);

        if (row is null)
        {
            row = new ProbationEvaluation
            {
                Id = Guid.NewGuid(),
                EmployeeId = employeeId,
                EmployeeCode = employeeCode,
                ProbationEndDate = probationEndDate
            };
            db.ProbationEvaluations.Add(row);
        }

        row.ProbationEndDate = probationEndDate;
        row.Status = ProbationEvaluationStatus.Decided;
        row.DecidedOutcomeCode = outcomeCode;
        row.DecidedByIdpSubject = decidedByIdpSubject;
        row.DecidedAtUtc = DateTime.UtcNow;
        row.DecisionNote = note;
        row.ExtendDurationCode = extendDurationCode;
        if (!string.IsNullOrWhiteSpace(criteriaPayloadJson))
            row.CriteriaPayloadJson = criteriaPayloadJson;

        await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Map(row);
    }

    private static ProbationEvaluationSnapshot Map(ProbationEvaluation x) =>
        new(
            x.Id,
            x.EmployeeId,
            x.EmployeeCode,
            x.ProbationEndDate,
            x.Status,
            x.ProposedOutcomeCode,
            x.ProposedByIdpSubject,
            x.ProposedAtUtc,
            x.ProposedNote,
            x.CriteriaPayloadJson,
            x.DecidedOutcomeCode,
            x.DecidedByIdpSubject,
            x.DecidedAtUtc,
            x.DecisionNote,
            x.ExtendDurationCode);
}
