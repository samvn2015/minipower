using Hrm.Application.Common;
using Hrm.Domain.Employees.Repositories;
using Hrm.Domain.Identity.Repositories;
using Hrm.Domain.Shared.Constants;
using Jarvis.Domain.Shared.ExceptionHandling;

namespace Hrm.Application.Leave;

public static class LevEmployeeGuard
{
    public static async Task<(IdentityAccountSnapshot Actor, EmployeeSnapshot Employee)> ResolveActorEmployeeAsync(
        IIdentityAccountReadRepository accounts,
        IEmployeeReadRepository employees,
        string? actorIdpSubject,
        CancellationToken cancellationToken)
    {
        IamAccessGuard.RequireAuthenticated(actorIdpSubject);

        var actor = await accounts.FindByIdpSubjectAsync(actorIdpSubject!, cancellationToken)
            .ConfigureAwait(false);
        if (actor is null || actor.Status != Domain.Identity.IdentityAccountStatus.Active)
            throw new ForbiddenException(HrmErrorCodes.Forbidden, "Tài khoản không hiệu lực.");

        if (string.IsNullOrWhiteSpace(actor.EmployeeCode))
        {
            throw new NotFoundException(
                HrmErrorCodes.NotFound,
                "Chưa liên kết MNV — không có hồ sơ EMP (IAM-FR-017).");
        }

        var employee = await employees
            .FindByEmployeeCodeAsync(actor.EmployeeCode, cancellationToken)
            .ConfigureAwait(false)
            ?? throw new NotFoundException(
                HrmErrorCodes.NotFound,
                $"Employee {actor.EmployeeCode} không tồn tại.");

        return (actor, employee);
    }
}
