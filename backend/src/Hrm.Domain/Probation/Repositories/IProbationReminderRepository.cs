using Hrm.Domain.Shared.Paging;

namespace Hrm.Domain.Probation.Repositories;

public sealed record ProbationReminderCreateModel(
    ProbationReminderKind Kind,
    Guid EmployeeId,
    string EmployeeCode,
    DateOnly ProbationEndDate,
    DateOnly DueDate,
    DateOnly AsOfDate,
    Guid? AssigneeEmployeeId,
    string? AssigneeEmployeeCode,
    string InAppMessage,
    string EmailTo,
    string Channel,
    string CreatedByIdpSubject);

public sealed record ProbationReminderSnapshot(
    Guid Id,
    ProbationReminderKind Kind,
    Guid EmployeeId,
    string EmployeeCode,
    DateOnly ProbationEndDate,
    DateOnly DueDate,
    DateOnly AsOfDate,
    Guid? AssigneeEmployeeId,
    string? AssigneeEmployeeCode,
    string InAppMessage,
    string EmailTo,
    string Channel,
    DateTime CreatedAtUtc);

public interface IProbationReminderRepository
{
    Task<bool> ExistsAsync(
        Guid employeeId,
        ProbationReminderKind kind,
        DateOnly probationEndDate,
        CancellationToken cancellationToken = default);

    Task AddManyAsync(
        IReadOnlyList<ProbationReminderCreateModel> rows,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// S1 — danh sách nhắc tích luỹ theo mỗi lần thử việc, không bao giờ xoá → phân trang.
    /// Không có bản không giới hạn: job T-15/T-7 dùng <see cref="ExistsAsync"/> +
    /// <see cref="AddManyAsync"/>, không duyệt danh sách này.
    /// </summary>
    Task<PagedResult<ProbationReminderSnapshot>> ListPagedAsync(
        ProbationReminderKind? kind,
        PageRequest page,
        CancellationToken cancellationToken = default);
}
