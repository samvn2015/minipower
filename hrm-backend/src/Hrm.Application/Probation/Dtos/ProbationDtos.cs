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
