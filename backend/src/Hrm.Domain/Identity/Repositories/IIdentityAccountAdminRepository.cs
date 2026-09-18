using Hrm.Domain.Identity;

namespace Hrm.Domain.Identity.Repositories;

public interface IIdentityAccountAdminRepository
{
    Task<IReadOnlyList<IdentityAccountSnapshot>> ListAsync(CancellationToken cancellationToken = default);

    Task<IdentityAccountSnapshot?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <returns><c>true</c> nếu có thay đổi thật; <c>false</c> = no-op (đã có / không có / không tồn tại) — caller KHÔNG audit.</returns>
    Task<bool> AssignRoleAsync(Guid accountId, string roleCode, CancellationToken cancellationToken = default);

    /// <returns><c>true</c> nếu có thay đổi thật; <c>false</c> = no-op (đã có / không có / không tồn tại) — caller KHÔNG audit.</returns>
    Task<bool> RemoveRoleAsync(Guid accountId, string roleCode, CancellationToken cancellationToken = default);

    /// <returns><c>true</c> nếu có thay đổi thật; <c>false</c> = no-op (đã có / không có / không tồn tại) — caller KHÔNG audit.</returns>
    Task<bool> SetStatusAsync(Guid accountId, IdentityAccountStatus status, CancellationToken cancellationToken = default);
}
