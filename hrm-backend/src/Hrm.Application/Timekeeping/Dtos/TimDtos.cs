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

public sealed record TimesheetImportRowDto(
    int RowNumber,
    string? EmployeeCode,
    decimal? WorkDays,
    decimal? Ot15,
    decimal? Ot20,
    decimal? Ot30,
    bool IsOk,
    string? ErrorCode,
    string? ErrorMessage);

public sealed record TimesheetImportBatchDto(
    Guid Id,
    string PeriodYm,
    string TemplateVersionCode,
    string Status,
    int TotalRows,
    int ErrorRows,
    bool HasMustErrors,
    string? FileName,
    IReadOnlyList<TimesheetImportRowDto> Rows);

public sealed record TimesheetImportPreviewResult(Guid BatchId, bool HasMustErrors, int TotalRows, int ErrorRows);

public sealed record TimesheetCommitResult(
    Guid PeriodId,
    string PeriodYm,
    string Status,
    int LineCount);
