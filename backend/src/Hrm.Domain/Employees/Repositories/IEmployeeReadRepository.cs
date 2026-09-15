using Hrm.Domain.Shared.Paging;

namespace Hrm.Domain.Employees.Repositories;

public interface IEmployeeReadRepository
{
    /// <summary>
    /// Đọc TOÀN BỘ — dành cho job duyệt hết nhân viên (nhắc T-15/T-7, quét PRB).
    /// **Không** dùng cho endpoint API: xem <see cref="ListPagedAsync"/> (code-review S1).
    /// </summary>
    Task<IReadOnlyList<EmployeeSnapshot>> ListAsync(CancellationToken cancellationToken = default);

    /// <summary>Một trang danh sách nhân viên + tổng số dòng (S1).</summary>
    Task<PagedResult<EmployeeSnapshot>> ListPagedAsync(
        PageRequest page,
        CancellationToken cancellationToken = default);

    Task<EmployeeSnapshot?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<EmployeeSnapshot?> FindByEmployeeCodeAsync(
        string employeeCode,
        CancellationToken cancellationToken = default);

    Task<EmployeeSnapshot?> FindByEmailCtyAsync(
        string emailCty,
        CancellationToken cancellationToken = default);

    Task<EmployeeUniqueField?> FindDuplicateAsync(
        string employeeCode,
        string? cccd,
        string? emailCty,
        string? taxId,
        Guid? excludeEmployeeId = null,
        CancellationToken cancellationToken = default);
}
