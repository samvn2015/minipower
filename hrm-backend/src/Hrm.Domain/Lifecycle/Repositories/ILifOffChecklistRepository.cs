namespace Hrm.Domain.Lifecycle.Repositories;

public sealed record LifOffChecklistItemSnapshot(
    string Code,
    string Name,
    bool IsMust,
    int SortOrder);

public sealed record LifOffChecklistTickSnapshot(
    string ItemCode,
    bool IsChecked,
    string? CheckedByIdpSubject,
    DateTime? CheckedAtUtc);

public interface ILifOffChecklistRepository
{
    Task<IReadOnlyList<LifOffChecklistItemSnapshot>> ListActiveItemsAsync(
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<LifOffChecklistTickSnapshot>> ListTicksAsync(
        Guid caseId,
        CancellationToken cancellationToken = default);

    Task UpsertTickAsync(
        Guid caseId,
        string itemCode,
        bool isChecked,
        string actorIdpSubject,
        CancellationToken cancellationToken = default);

    Task<bool> AllMustCheckedAsync(Guid caseId, CancellationToken cancellationToken = default);
}
