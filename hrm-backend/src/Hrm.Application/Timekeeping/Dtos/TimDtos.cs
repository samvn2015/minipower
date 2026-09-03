namespace Hrm.Application.Timekeeping.Dtos;

public sealed record TimesheetTemplateColumnDto(
    string ColumnKey,
    string DisplayName,
    int SortOrder,
    bool IsRequired,
    string MapsTo);

public sealed record TimesheetTemplateDto(
    Guid Id,
    string VersionCode,
    string Name,
    string Status,
    DateTime? PublishedAtUtc,
    string? PublishedByIdpSubject,
    IReadOnlyList<TimesheetTemplateColumnDto> Columns);

public sealed record TimesheetTemplateCreateResult(Guid Id, string VersionCode, string Status);

public sealed record TimesheetTemplatePublishResult(Guid Id, string VersionCode, string Status);
