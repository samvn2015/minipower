using Hrm.Domain.Identity.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Hrm.Infrastructure.Persistence.Repositories;

internal sealed class IdentityAccountReadRepository(AppDbContext db) : IIdentityAccountReadRepository
{
    public async Task<IdentityAccountSnapshot?> FindByIdpSubjectAsync(
        string idpSubject,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(idpSubject))
            return null;

        return await db.IdentityAccounts
            .AsNoTracking()
            .Where(a => a.IdpSubject == idpSubject)
            .Select(a => new IdentityAccountSnapshot(
                a.Id,
                a.IdpSubject,
                a.DisplayName,
                a.EmailCty,
                a.EmployeeCode,
                a.Status,
                a.AccountRoles.Select(ar => ar.RoleCode).ToList()))
            .FirstOrDefaultAsync(cancellationToken);
    }
}
