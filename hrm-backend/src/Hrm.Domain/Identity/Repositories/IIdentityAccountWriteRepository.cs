namespace Hrm.Domain.Identity.Repositories;

public interface IIdentityAccountWriteRepository
{
    Task<IdentityAccountSnapshot> CreateAsync(
        IdentityAccountCreateModel model,
        CancellationToken cancellationToken = default);
}
