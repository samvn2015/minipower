namespace Hrm.Application.Probation.Dtos;

public sealed record ProbationCaseDto(
    Guid EmployeeId,
    string EmployeeCode,
    string? FullName,
    string ContractType,
    DateOnly ProbationStartDate,
    DateOnly? ProbationEndDate,
    bool HasCompleteMilestone,
    DateOnly? T15DueDate,
    DateOnly? T7DueDate);

public sealed record ProbationMilestoneDto(
    Guid EmployeeId,
    string EmployeeCode,
    string? FullName,
    string? ContractType,
    DateOnly? ProbationStartDate,
    DateOnly? ProbationEndDate,
    bool IsOnProbation,
    bool HasCompleteMilestone,
    DateOnly? T15DueDate,
    DateOnly? T7DueDate,
    string Source);

public sealed record ProbationReminderDto(
    Guid Id,
    string Kind,
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

public sealed record ProbationMasterItemDto(string Code, string Name, int SortOrder);

public sealed record ProbationExtendDurationDto(string Code, string Name, int Months, int SortOrder);

public sealed record ProbationEvaluationDto(
    Guid Id,
    Guid EmployeeId,
    string EmployeeCode,
    DateOnly ProbationEndDate,
    string Status,
    string? ProposedOutcomeCode,
    string? ProposedByIdpSubject,
    DateTime? ProposedAtUtc,
    string? ProposedNote,
    string? CriteriaPayloadJson,
    string? DecidedOutcomeCode,
    string? DecidedByIdpSubject,
    DateTime? DecidedAtUtc,
    string? DecisionNote,
    string? ExtendDurationCode,
    bool ContractConvertedToOfficial = false,
    DateOnly? NewProbationEndDate = null,
    Guid? LifOffboardingCaseId = null);
