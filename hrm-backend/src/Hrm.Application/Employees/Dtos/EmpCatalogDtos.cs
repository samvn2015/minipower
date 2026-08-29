namespace Hrm.Application.Employees.Dtos;

public sealed record SeniorityDto(
    int Years,
    int Months,
    string DisplayText,
    string RuleCode);

public sealed record EducationLevelDto(string Code, string Name);

public sealed record EmpAuditLogDto(
    Guid Id,
    string Action,
    Guid? EmployeeId,
    Guid? RelatedId,
    string ActorIdpSubject,
    DateTime OccurredAtUtc,
    string? Detail);
