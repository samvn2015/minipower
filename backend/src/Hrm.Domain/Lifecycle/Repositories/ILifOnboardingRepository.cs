using Hrm.Domain.Lifecycle;

namespace Hrm.Domain.Lifecycle.Repositories;

public sealed record LifOnboardingCreateModel(
    Guid EmployeeId,
    string EmployeeCode,
    string CreatedByIdpSubject,
    string? Note);

public sealed record LifOnboardingSnapshot(
    Guid Id,
    Guid EmployeeId,
    string EmployeeCode,
    LifOnboardingStatus Status,
    DateTime CreatedAtUtc,
    string CreatedByIdpSubject,
    string? Note,
    bool EmailCtyProvisioned,
    bool GitProvisioned,
    bool CrmSpProvisioned,
    bool ChatProvisioned,
    DateTime? EmailCtyProvisionedAtUtc,
    DateTime? GitProvisionedAtUtc,
    DateTime? CrmSpProvisionedAtUtc,
    DateTime? ChatProvisionedAtUtc,
    string? ClosedByIdpSubject,
    DateTime? ClosedAtUtc);

public sealed record LifOnChecklistItemSnapshot(string Code, string Name, bool IsMust, int SortOrder);

public sealed record LifOnChecklistTickSnapshot(
    string ItemCode,
    bool IsChecked,
    string? CheckedByIdpSubject,
    DateTime? CheckedAtUtc);

public interface ILifOnboardingRepository
{
    Task<LifOnboardingSnapshot> CreateAsync(
        LifOnboardingCreateModel model,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<LifOnboardingSnapshot>> ListAsync(CancellationToken cancellationToken = default);

    Task<LifOnboardingSnapshot?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<LifOnboardingSnapshot> MarkProvisionedAsync(
        Guid id,
        string systemCode,
        string actorIdpSubject,
        CancellationToken cancellationToken = default);

    Task<LifOnboardingSnapshot> CloseAsync(
        Guid id,
        string closedByIdpSubject,
        CancellationToken cancellationToken = default);
}

public interface ILifOnChecklistRepository
{
    Task<IReadOnlyList<LifOnChecklistItemSnapshot>> ListActiveItemsAsync(
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<LifOnChecklistTickSnapshot>> ListTicksAsync(
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
