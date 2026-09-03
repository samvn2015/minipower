namespace Hrm.Domain.Timekeeping.Repositories;

public sealed record TimesheetImportRowSnapshot(
    Guid Id,
    int RowNumber,
    string? EmployeeCode,
    Guid? EmployeeId,
    decimal? WorkDays,
    decimal? Ot15,
    decimal? Ot20,
    decimal? Ot30,
    decimal? OtUnclassified,
    bool IsOk,
    string? ErrorCode,
    string? ErrorMessage);

public sealed record TimesheetImportBatchSnapshot(
    Guid Id,
    string PeriodYm,
    Guid TemplateVersionId,
    string TemplateVersionCode,
    TimesheetImportBatchStatus Status,
    string UploadedByIdpSubject,
    DateTime UploadedAtUtc,
    string? FileName,
    int TotalRows,
    int ErrorRows,
    bool HasMustErrors,
    IReadOnlyList<TimesheetImportRowSnapshot> Rows);

public sealed record TimesheetImportRowCreateModel(
    int RowNumber,
    string? EmployeeCode,
    Guid? EmployeeId,
    decimal? WorkDays,
    decimal? Ot15,
    decimal? Ot20,
    decimal? Ot30,
    decimal? OtUnclassified,
    bool IsOk,
    string? ErrorCode,
    string? ErrorMessage);

public sealed record TimesheetImportBatchCreateModel(
    string PeriodYm,
    Guid TemplateVersionId,
    string TemplateVersionCode,
    string UploadedByIdpSubject,
    string? FileName,
    IReadOnlyList<TimesheetImportRowCreateModel> Rows);

public sealed record TimesheetLineSnapshot(
    Guid Id,
    Guid EmployeeId,
    string EmployeeCode,
    decimal WorkDays,
    decimal Ot15,
    decimal Ot20,
    decimal Ot30,
    decimal OtUnclassified);

public sealed record TimesheetPeriodSnapshot(
    Guid Id,
    string PeriodYm,
    TimesheetPeriodStatus Status,
    Guid? SourceImportBatchId,
    int LineCount,
    IReadOnlyList<TimesheetLineSnapshot> Lines);

public interface ITimesheetImportRepository
{
    Task<Guid> CreatePreviewAsync(
        TimesheetImportBatchCreateModel model,
        CancellationToken cancellationToken = default);

    Task<TimesheetImportBatchSnapshot?> FindBatchByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<TimesheetPeriodSnapshot?> FindPeriodByYmAsync(
        string periodYm,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TimesheetPeriodSnapshot>> ListPeriodsAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Commit preview → Period Draft + lines. Returns null if batch missing / already committed / has Must errors / period Closed.
    /// </summary>
    Task<TimesheetPeriodSnapshot?> CommitAsync(
        Guid batchId,
        string committedByIdpSubject,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Close Draft period. Returns null if missing / not Draft.
    /// Caller must validate OT unclassified before calling.
    /// </summary>
    Task<TimesheetPeriodSnapshot?> ClosePeriodAsync(
        string periodYm,
        string closedByIdpSubject,
        CancellationToken cancellationToken = default);
}
