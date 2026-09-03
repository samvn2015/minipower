namespace Hrm.Domain.Timekeeping.Repositories;

public sealed record TimesheetTemplateColumnSnapshot(
    Guid Id,
    string ColumnKey,
    string DisplayName,
    int SortOrder,
    bool IsRequired,
    string MapsTo);

public sealed record TimesheetTemplateVersionSnapshot(
    Guid Id,
    string VersionCode,
    string Name,
    TimesheetTemplateStatus Status,
    DateTime? PublishedAtUtc,
    string? PublishedByIdpSubject,
    IReadOnlyList<TimesheetTemplateColumnSnapshot> Columns);

public sealed record TimesheetTemplateColumnCreateModel(
    string ColumnKey,
    string DisplayName,
    int SortOrder,
    bool IsRequired,
    string MapsTo);

public sealed record TimesheetTemplateCreateModel(
    string VersionCode,
    string Name,
    IReadOnlyList<TimesheetTemplateColumnCreateModel> Columns);

public interface ITimesheetTemplateRepository
{
    Task<TimesheetTemplateVersionSnapshot?> FindActiveAsync(CancellationToken cancellationToken = default);

    Task<TimesheetTemplateVersionSnapshot?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TimesheetTemplateVersionSnapshot>> ListAsync(CancellationToken cancellationToken = default);

    Task<Guid> CreateDraftAsync(TimesheetTemplateCreateModel model, CancellationToken cancellationToken = default);

    Task<bool> ExistsByVersionCodeAsync(string versionCode, CancellationToken cancellationToken = default);

    /// <summary>Atomic: retire previous Active, set target Active. Returns false if not found / not Draft.</summary>
    Task<bool> PublishAsync(
        Guid id,
        string publishedByIdpSubject,
        CancellationToken cancellationToken = default);

    Task<int> CountActiveAsync(CancellationToken cancellationToken = default);
}
