using Hrm.Domain.Identity;
using Hrm.Domain.Identity.Entities;
using Hrm.Domain.Identity.Repositories;
using Hrm.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Hrm.Infrastructure.Persistence.Repositories;

internal sealed class IdentityAccountWriteRepository(AppDbContext db, IAppUnitOfWork unitOfWork)
    : IIdentityAccountWriteRepository
{
    public async Task<IdentityAccountSnapshot> CreateAsync(
        IdentityAccountCreateModel model,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(model);

        var accountId = Guid.NewGuid();
        var account = new IdentityAccount
        {
            Id = accountId,
            IdpSubject = model.IdpSubject.Trim(),
            DisplayName = model.DisplayName?.Trim(),
            EmailCty = model.EmailCty?.Trim(),
            EmployeeCode = model.EmployeeCode.Trim(),
            Status = IdentityAccountStatus.Active
        };

        db.IdentityAccounts.Add(account);

        foreach (var roleCode in model.InitialRoleCodes
                     .Where(static r => !string.IsNullOrWhiteSpace(r))
                     .Distinct(StringComparer.OrdinalIgnoreCase))
        {
            db.AccountRoles.Add(new AccountRole
            {
                AccountId = accountId,
                RoleCode = roleCode.Trim()
            });
        }

        try
        {
            await unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (DbUpdateException)
        {
            var raced = await db.IdentityAccounts
                .AsNoTracking()
                .Where(a => a.IdpSubject == model.IdpSubject.Trim())
                .Select(a => new IdentityAccountSnapshot(
                    a.Id,
                    a.IdpSubject,
                    a.DisplayName,
                    a.EmailCty,
                    a.EmployeeCode,
                    a.Status,
                    a.AccountRoles.Select(ar => ar.RoleCode).ToList()))
                .FirstOrDefaultAsync(cancellationToken);
            if (raced is not null)
                return raced;

            throw;
        }

        return await db.IdentityAccounts
            .AsNoTracking()
            .Where(a => a.Id == accountId)
            .Select(a => new IdentityAccountSnapshot(
                a.Id,
                a.IdpSubject,
                a.DisplayName,
                a.EmailCty,
                a.EmployeeCode,
                a.Status,
                a.AccountRoles.Select(ar => ar.RoleCode).ToList()))
            .FirstAsync(cancellationToken);
    }
}
