using Hrm.Domain.Probation;

namespace Hrm.Domain.Probation.Repositories;

public sealed record ProbationOutcomeSnapshot(string Code, string Name, int SortOrder);

public sealed record ProbationCriterionSnapshot(string Code, string Name, int SortOrder);

public sealed record ProbationExtendDurationSnapshot(string Code, string Name, int Months, int SortOrder);

public sealed record ProbationEvaluationSnapshot(
    Guid Id,
    Guid EmployeeId,
    string EmployeeCode,
    DateOnly ProbationEndDate,
    ProbationEvaluationStatus Status,
    string? ProposedOutcomeCode,
    string? ProposedByIdpSubject,
    DateTime? ProposedAtUtc,
    string? ProposedNote,
    string? CriteriaPayloadJson,
    string? DecidedOutcomeCode,
    string? DecidedByIdpSubject,
    DateTime? DecidedAtUtc,
    string? DecisionNote,
    string? ExtendDurationCode);

public interface IProbationMasterReadRepository
{
    Task<IReadOnlyList<ProbationOutcomeSnapshot>> ListOutcomesAsync(CancellationToken cancellationToken = default);

    Task<bool> OutcomeExistsAsync(string code, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ProbationCriterionSnapshot>> ListCriteriaAsync(CancellationToken cancellationToken = default);

    Task<bool> CriterionExistsAsync(string code, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ProbationExtendDurationSnapshot>> ListExtendDurationsAsync(
        CancellationToken cancellationToken = default);

    Task<ProbationExtendDurationSnapshot?> FindExtendDurationAsync(
        string code,
        CancellationToken cancellationToken = default);
}

public interface IProbationEvaluationRepository
{
    Task<ProbationEvaluationSnapshot?> FindOpenByEmployeeAsync(
        Guid employeeId,
        CancellationToken cancellationToken = default);

    Task<ProbationEvaluationSnapshot?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ProbationEvaluationSnapshot>> ListAsync(CancellationToken cancellationToken = default);

    Task<ProbationEvaluationSnapshot> UpsertProposeAsync(
        Guid employeeId,
        string employeeCode,
        DateOnly probationEndDate,
        string outcomeCode,
        string proposedByIdpSubject,
        string? note,
        string? criteriaPayloadJson,
        CancellationToken cancellationToken = default);

    Task<ProbationEvaluationSnapshot> DecideAsync(
        Guid employeeId,
        string employeeCode,
        DateOnly probationEndDate,
        string outcomeCode,
        string decidedByIdpSubject,
        string? note,
        string? extendDurationCode,
        string? criteriaPayloadJson,
        CancellationToken cancellationToken = default);
}
