namespace Hrm.Domain.Shared.Paging;

/// <summary>
/// Một trang dữ liệu kèm **tổng số dòng** — tổng đi qua header <c>X-Total-Count</c>
/// chứ không bọc vào body, để response vẫn là mảng và frontend hiện tại không phải sửa.
/// </summary>
public sealed record PagedResult<T>(IReadOnlyList<T> Items, int Total);
