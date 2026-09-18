using Hrm.Application.Common;
using Hrm.Application.Identity.Admin.Commands;
using Hrm.Domain.Employees;
using Hrm.Domain.Employees.Repositories;
using Hrm.Domain.Identity;
using Hrm.Domain.Identity.Constants;
using Hrm.Domain.Identity.Repositories;
using Hrm.Domain.Shared.Constants;
using Jarvis.Application.Contracts.Commands;
using Jarvis.Domain.Shared.ExceptionHandling;

namespace Hrm.Application.Identity.Admin.Commands;

public sealed class AssignAccountRoleCommandHandler(
    IIdentityAccountReadRepository accounts,
    IIdentityAccountAdminRepository admin,
    IIamAuditLogRepository auditLogs,
    IIamAtomicScope scope)
    : IAsyncCommandHandler<AssignAccountRoleCommand, IdentityAccountAdminResult>
{
    public async Task<IdentityAccountAdminResult> HandleAsync(
        AssignAccountRoleCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);
        await AuthorizeHrOrItAsync(command.ActorIdpSubject, cancellationToken).ConfigureAwait(false);
        ValidateRoleCode(command.RoleCode);

        // M1/M2 (pass 4): một transaction; chỉ audit khi có thay đổi thật (không audit no-op).
        return await scope.RunAsync(async ct =>
        {
            var changed = await admin.AssignRoleAsync(command.AccountId, command.RoleCode, ct)
                .ConfigureAwait(false);

            if (changed)
            {
                await auditLogs.AppendAsync(
                        new EmpAuditLogEntry(
                            EmpAuditActions.IamRoleAssigned,
                            null,
                            command.AccountId,
                            command.ActorIdpSubject!,
                            $"Role={command.RoleCode}"),
                        ct)
                    .ConfigureAwait(false);
            }

            return await ToResultAsync(command.AccountId, ct).ConfigureAwait(false);
        }, cancellationToken).ConfigureAwait(false);
    }

    internal static void ValidateRoleCode(string roleCode)
    {
        if (string.IsNullOrWhiteSpace(roleCode) || !IamRoleCodes.All.Contains(roleCode))
            throw new BadRequestException(HrmErrorCodes.BadRequest, $"RoleCode không hợp lệ: {roleCode}");
    }

    internal static async Task AuthorizeHrOrItAsync(
        string? actorIdpSubject,
        CancellationToken cancellationToken,
        IIdentityAccountReadRepository accounts)
    {
        IamAccessGuard.RequireAuthenticated(actorIdpSubject);
        var actor = await accounts.FindByIdpSubjectAsync(actorIdpSubject!, cancellationToken).ConfigureAwait(false);
        IamAccessGuard.RequireHrOrIt(actor);
    }

    private Task AuthorizeHrOrItAsync(string? actorIdpSubject, CancellationToken cancellationToken) =>
        AuthorizeHrOrItAsync(actorIdpSubject, cancellationToken, accounts);

    private async Task<IdentityAccountAdminResult> ToResultAsync(
        Guid accountId,
        CancellationToken cancellationToken)
    {
        var account = await admin.FindByIdAsync(accountId, cancellationToken).ConfigureAwait(false)
            ?? throw new NotFoundException(HrmErrorCodes.NotFound, $"IdentityAccount {accountId} không tồn tại.");

        return new IdentityAccountAdminResult(
            account.AccountId,
            account.Status.ToString(),
            account.RoleCodes);
    }
}

public sealed class RemoveAccountRoleCommandHandler(
    IIdentityAccountReadRepository accounts,
    IIdentityAccountAdminRepository admin,
    IIamAuditLogRepository auditLogs,
    IIamAtomicScope scope)
    : IAsyncCommandHandler<RemoveAccountRoleCommand, IdentityAccountAdminResult>
{
    public async Task<IdentityAccountAdminResult> HandleAsync(
        RemoveAccountRoleCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);
        await AssignAccountRoleCommandHandler.AuthorizeHrOrItAsync(
            command.ActorIdpSubject, cancellationToken, accounts).ConfigureAwait(false);
        AssignAccountRoleCommandHandler.ValidateRoleCode(command.RoleCode);

        return await scope.RunAsync(async ct =>
        {
            var changed = await admin.RemoveRoleAsync(command.AccountId, command.RoleCode, ct)
                .ConfigureAwait(false);

            if (changed)
            {
                await auditLogs.AppendAsync(
                        new EmpAuditLogEntry(
                            EmpAuditActions.IamRoleRemoved,
                            null,
                            command.AccountId,
                            command.ActorIdpSubject!,
                            $"Role={command.RoleCode}"),
                        ct)
                    .ConfigureAwait(false);
            }

            var account = await admin.FindByIdAsync(command.AccountId, ct).ConfigureAwait(false)
                ?? throw new NotFoundException(HrmErrorCodes.NotFound, $"IdentityAccount {command.AccountId} không tồn tại.");

            return new IdentityAccountAdminResult(
                account.AccountId,
                account.Status.ToString(),
                account.RoleCodes);
        }, cancellationToken).ConfigureAwait(false);
    }
}

public sealed class DisableIdentityAccountCommandHandler(
    IIdentityAccountReadRepository accounts,
    IIdentityAccountAdminRepository admin,
    IIamAuditLogRepository auditLogs,
    IIamAtomicScope scope)
    : IAsyncCommandHandler<DisableIdentityAccountCommand, IdentityAccountAdminResult>
{
    public async Task<IdentityAccountAdminResult> HandleAsync(
        DisableIdentityAccountCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);
        IamAccessGuard.RequireAuthenticated(command.ActorIdpSubject);

        var actor = await accounts.FindByIdpSubjectAsync(command.ActorIdpSubject!, cancellationToken)
            .ConfigureAwait(false);
        IamAccessGuard.RequireIt(actor);

        return await scope.RunAsync(async ct =>
        {
            // Tài khoản không tồn tại → 404 TRƯỚC khi có bất kỳ dòng audit nào (M2).
            var account = await admin.FindByIdAsync(command.AccountId, ct).ConfigureAwait(false)
                ?? throw new NotFoundException(HrmErrorCodes.NotFound, $"IdentityAccount {command.AccountId} không tồn tại.");

            var changed = await admin.SetStatusAsync(command.AccountId, IdentityAccountStatus.Disabled, ct)
                .ConfigureAwait(false);

            if (changed)
            {
                await auditLogs.AppendAsync(
                        new EmpAuditLogEntry(
                            EmpAuditActions.IamAccountDisabled,
                            null,
                            command.AccountId,
                            command.ActorIdpSubject!,
                            $"FromStatus={account.Status}"),
                        ct)
                    .ConfigureAwait(false);
            }

            return new IdentityAccountAdminResult(
                account.AccountId,
                IdentityAccountStatus.Disabled.ToString(),
                account.RoleCodes);
        }, cancellationToken).ConfigureAwait(false);
    }
}
