namespace Hrm.Application.Lifecycle.Dtos;

public sealed record LifOffboardingDto(
    Guid Id,
    Guid EmployeeId,
    string EmployeeCode,
    string Source,
    string Status,
    DateOnly? LastWorkingDayN,
    DateOnly? NPlus3Expected,
    DateOnly? ResignationSignedDate,
    bool JobNPlus3Eligible,
    string? ConfirmedByIdpSubject,
    DateTime? ConfirmedAtUtc,
    DateTime CreatedAtUtc,
    string CreatedByIdpSubject,
    string? Note);
