using Hrm.Domain.Identity;
using Hrm.Domain.Identity.Entities;
using Hrm.Domain.Identity.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Hrm.Infrastructure.Persistence.Repositories;

internal sealed class IdentityAccountAdminRepository(AppDbContext db)
    : IIdentityAccountAdminRepository
{
    public async Task<IReadOnlyList<IdentityAccountSnapshot>> ListAsync(
        CancellationToken cancellationToken = default)
    {
        return await db.IdentityAccounts
            .AsNoTracking()
            .OrderBy(a => a.DisplayName)
            .Select(a => new IdentityAccountSnapshot(
                a.Id,
                a.IdpSubject,
                a.DisplayName,
                a.EmailCty,
                a.EmployeeCode,
                a.Status,
                a.AccountRoles.Select(ar => ar.RoleCode).ToList()))
            .ToListAsync(cancellationToken);
    }

    public async Task<IdentityAccountSnapshot?> FindByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await db.IdentityAccounts
            .AsNoTracking()
            .Where(a => a.Id == id)
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

    public async Task AssignRoleAsync(
        Guid accountId,
        string roleCode,
        CancellationToken cancellationToken = default)
    {
        var accountExists = await db.IdentityAccounts.AnyAsync(a => a.Id == accountId, cancellationToken);
        if (!accountExists)
            throw new InvalidOperationException($"IdentityAccount {accountId} không tồn tại.");

        var exists = await db.AccountRoles
            .AnyAsync(
                ar => ar.AccountId == accountId
                      && ar.RoleCode == roleCode,
                cancellationToken);
        if (exists)
            return;

        var roleExists = await db.Roles.AnyAsync(r => r.RoleCode == roleCode, cancellationToken);
        if (!roleExists)
            throw new InvalidOperationException($"Role {roleCode} không tồn tại.");

        db.AccountRoles.Add(new AccountRole
        {
            AccountId = accountId,
            RoleCode = roleCode
        });

        await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task RemoveRoleAsync(
        Guid accountId,
        string roleCode,
        CancellationToken cancellationToken = default)
    {
        var link = await db.AccountRoles
            .FirstOrDefaultAsync(
                ar => ar.AccountId == accountId && ar.RoleCode == roleCode,
                cancellationToken);
        if (link is null)
            return;

        db.AccountRoles.Remove(link);
        await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task SetStatusAsync(
        Guid accountId,
        IdentityAccountStatus status,
        CancellationToken cancellationToken = default)
    {
        var account = await db.IdentityAccounts
            .FirstOrDefaultAsync(a => a.Id == accountId, cancellationToken);
        if (account is null)
            return;

        account.Status = status;
        await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}
