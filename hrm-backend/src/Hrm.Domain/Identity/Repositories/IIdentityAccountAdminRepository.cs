using Hrm.Domain.Identity;

namespace Hrm.Domain.Identity.Repositories;

public interface IIdentityAccountAdminRepository
{
    Task<IReadOnlyList<IdentityAccountSnapshot>> ListAsync(CancellationToken cancellationToken = default);

    Task<IdentityAccountSnapshot?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task AssignRoleAsync(Guid accountId, string roleCode, CancellationToken cancellationToken = default);

    Task RemoveRoleAsync(Guid accountId, string roleCode, CancellationToken cancellationToken = default);

    Task SetStatusAsync(Guid accountId, IdentityAccountStatus status, CancellationToken cancellationToken = default);
}
