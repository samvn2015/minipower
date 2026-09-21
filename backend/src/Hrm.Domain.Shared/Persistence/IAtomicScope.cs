namespace Hrm.Domain.Shared.Persistence;

/// <summary>
/// Bọc một chuỗi thao tác (nghiệp vụ + outbox + audit) vào <b>một</b> transaction DB của
/// bounded context. Doc-review pass 4 M1: trước đây mỗi repository tự <c>SaveChanges</c>
/// nên audit nằm ở transaction riêng — nghiệp vụ commit mà audit fail là "sót audit",
/// trái NFR-005. Phương án A (DEC-ARC-024) chỉ đúng khi có lớp này.
///
/// Mỗi context một interface marker để DI trỏ đúng DbContext; test dùng fake chạy thẳng.
/// </summary>
public interface IAtomicScope
{
    /// <summary>Chạy <paramref name="work"/> trong một transaction; ném là rollback toàn bộ.</summary>
    Task<T> RunAsync<T>(Func<CancellationToken, Task<T>> work, CancellationToken cancellationToken = default);
}
