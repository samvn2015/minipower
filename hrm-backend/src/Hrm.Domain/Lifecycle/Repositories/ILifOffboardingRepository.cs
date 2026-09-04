using Hrm.Domain.Lifecycle;

namespace Hrm.Domain.Lifecycle.Repositories;

public sealed record LifOffboardingCreateModel(
    Guid EmployeeId,
    string EmployeeCode,
    string Source,
    string CreatedByIdpSubject,
    string? Note,
    DateOnly? ResignationSignedDate = null);

public sealed record LifOffboardingSnapshot(
    Guid Id,
    Guid EmployeeId,
    string EmployeeCode,
    string Source,
    LifOffboardingStatus Status,
    DateOnly? LastWorkingDayN,
    DateOnly? ResignationSignedDate,
    string? ConfirmedByIdpSubject,
    DateTime? ConfirmedAtUtc,
    DateTime CreatedAtUtc,
    string CreatedByIdpSubject,
    string? Note,
    DateTime? GitLockedAtUtc = null,
    DateTime? CrmSpLockedAtUtc = null,
    DateOnly? LockAsOfDate = null,
    bool IsEarlySecurityCr = false,
    string? EarlyCrReason = null,
    string? LockedByIdpSubject = null);

public sealed record LifAccessLockApplyModel(
    Guid CaseId,
    DateOnly AsOfDate,
    bool IsEarlySecurityCr,
    string? CrReason,
    string LockedByIdpSubject);

public interface ILifOffboardingRepository
{
    Task<LifOffboardingSnapshot> CreateAsync(
        LifOffboardingCreateModel model,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<LifOffboardingSnapshot>> ListOpenAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<LifOffboardingSnapshot>> ListAsync(CancellationToken cancellationToken = default);

    Task<LifOffboardingSnapshot?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<LifOffboardingSnapshot> ConfirmNAsync(
        Guid id,
        DateOnly lastWorkingDayN,
        string confirmedByIdpSubject,
        CancellationToken cancellationToken = default);

    Task<LifOffboardingSnapshot> CloseAsync(
        Guid id,
        string closedByIdpSubject,
        CancellationToken cancellationToken = default);

    /// <summary>Khóa Git + CRM SP cùng transaction + outbox (FR-005/006).</summary>
    Task<LifOffboardingSnapshot> ApplyAccessLocksAsync(
        LifAccessLockApplyModel model,
        CancellationToken cancellationToken = default);
}
