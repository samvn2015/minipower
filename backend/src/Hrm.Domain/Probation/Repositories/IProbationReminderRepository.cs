namespace Hrm.Domain.Probation.Repositories;

public sealed record ProbationReminderCreateModel(
    ProbationReminderKind Kind,
    Guid EmployeeId,
    string EmployeeCode,
    DateOnly ProbationEndDate,
    DateOnly DueDate,
    DateOnly AsOfDate,
    Guid? AssigneeEmployeeId,
    string? AssigneeEmployeeCode,
    string InAppMessage,
    string EmailTo,
    string Channel,
    string CreatedByIdpSubject);

public sealed record ProbationReminderSnapshot(
    Guid Id,
    ProbationReminderKind Kind,
    Guid EmployeeId,
    string EmployeeCode,
    DateOnly ProbationEndDate,
    DateOnly DueDate,
    DateOnly AsOfDate,
    Guid? AssigneeEmployeeId,
    string? AssigneeEmployeeCode,
    string InAppMessage,
    string EmailTo,
    string Channel,
    DateTime CreatedAtUtc);

public interface IProbationReminderRepository
{
    Task<bool> ExistsAsync(
        Guid employeeId,
        ProbationReminderKind kind,
        DateOnly probationEndDate,
        CancellationToken cancellationToken = default);

    Task AddManyAsync(
        IReadOnlyList<ProbationReminderCreateModel> rows,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ProbationReminderSnapshot>> ListAsync(
        ProbationReminderKind? kind = null,
        CancellationToken cancellationToken = default);
}
