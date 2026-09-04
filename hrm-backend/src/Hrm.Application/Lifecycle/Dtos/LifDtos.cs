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

public sealed record LifOffChecklistItemDto(
    string Code,
    string Name,
    bool IsMust,
    int SortOrder,
    bool IsChecked,
    string? CheckedByIdpSubject,
    DateTime? CheckedAtUtc);

public sealed record LifOffChecklistBoardDto(
    Guid CaseId,
    string Status,
    bool CanClose,
    IReadOnlyList<LifOffChecklistItemDto> Items);
