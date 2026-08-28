using Hrm.Domain.Employees;
using Hrm.Domain.Employees.Repositories;
using Hrm.Domain.Identity.Constants;
using Hrm.Domain.Identity.Repositories;

namespace Hrm.Application.Identity;

/// <summary>
/// IAM-FR-017 / IAM-BR-017 — first login: map IdP <c>sub</c> → EMP qua email công ty, gán <c>IAM-ROLE-NV</c>.
/// </summary>
public sealed class IdentityAccountProvisioner(
    IIdentityAccountReadRepository accounts,
    IIdentityAccountWriteRepository accountWrites,
    IEmployeeReadRepository employees)
{
    public async Task<IdentityProvisionResult> TryProvisionAsync(
        string idpSubject,
        string? emailCty,
        string? displayName,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(emailCty))
        {
            return IdentityProvisionResult.NotProvisioned(
                "Thiếu claim email — không auto-provision IAM (IAM-FR-017).");
        }

        var employee = await employees.FindByEmailCtyAsync(emailCty, cancellationToken)
            .ConfigureAwait(false);
        if (employee is null)
        {
            return IdentityProvisionResult.NotProvisioned(
                "Không tìm thấy EMP theo email — chưa tạo IdentityAccount (IAM-FR-017).");
        }

        if (employee.Status != EmployeeStatus.Active)
        {
            return IdentityProvisionResult.NotProvisioned(
                "EMP không hiệu lực — không auto-provision IAM (IAM-FR-017).");
        }

        var linkedAccount = await accounts.FindByEmployeeCodeAsync(employee.EmployeeCode, cancellationToken)
            .ConfigureAwait(false);
        if (linkedAccount is not null)
        {
            return IdentityProvisionResult.NotProvisioned(
                "MNV đã gắn tài khoản IAM khác — không auto-provision (IAM-FR-017).");
        }

        var created = await accountWrites.CreateAsync(
            new IdentityAccountCreateModel(
                idpSubject,
                displayName ?? employee.FullName,
                emailCty,
                employee.EmployeeCode,
                [IamRoleCodes.Nv]),
            cancellationToken).ConfigureAwait(false);

        return IdentityProvisionResult.Provisioned(created);
    }
}

public sealed record IdentityProvisionResult(
    IdentityAccountSnapshot? Account,
    string? Note)
{
    public static IdentityProvisionResult Provisioned(IdentityAccountSnapshot account) =>
        new(account, null);

    public static IdentityProvisionResult NotProvisioned(string note) =>
        new(null, note);
}
