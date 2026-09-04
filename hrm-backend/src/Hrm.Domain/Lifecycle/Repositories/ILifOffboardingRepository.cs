using Hrm.Domain.Lifecycle;

namespace Hrm.Domain.Lifecycle.Repositories;

public sealed record LifOffboardingCreateModel(
    Guid EmployeeId,
    string EmployeeCode,
    string Source,
    string CreatedByIdpSubject,
    string? Note);

public sealed record LifOffboardingSnapshot(
    Guid Id,
    Guid EmployeeId,
    string EmployeeCode,
    string Source,
    LifOffboardingStatus Status,
    DateOnly? LastWorkingDayN,
    DateTime CreatedAtUtc,
    string CreatedByIdpSubject,
    string? Note);

public interface ILifOffboardingRepository
{
    Task<LifOffboardingSnapshot> CreateAsync(
        LifOffboardingCreateModel model,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<LifOffboardingSnapshot>> ListOpenAsync(CancellationToken cancellationToken = default);

    Task<LifOffboardingSnapshot?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
}
