using Hrm.Domain.Identity;
using Hrm.Domain.Identity.Constants;
using Hrm.Domain.Identity.Repositories;
using Hrm.Domain.Shared.Constants;
using Jarvis.Domain.Shared.ExceptionHandling;

namespace Hrm.Application.Probation;

/// <summary>RBAC PRB slice A — HR/PGD xem hàng TV; NV chỉ mốc mình (SCR-004).</summary>
public static class PrbAccessGuard
{
    public static void RequireHrOrPgd(IdentityAccountSnapshot? actor)
    {
        if (actor is null || actor.Status != IdentityAccountStatus.Active)
            throw new ForbiddenException(HrmErrorCodes.Forbidden, "Không có quyền PRB.");

        if (!actor.RoleCodes.Any(static r =>
                string.Equals(r, IamRoleCodes.Hr, StringComparison.OrdinalIgnoreCase)
                || string.Equals(r, IamRoleCodes.Pgd, StringComparison.OrdinalIgnoreCase)))
        {
            throw new ForbiddenException(
                HrmErrorCodes.Forbidden,
                "Chỉ HR/PGD xem hàng thử việc (PRB-SCR-001).");
        }
    }

    public static void RequireAuthenticated(IdentityAccountSnapshot? actor)
    {
        if (actor is null || actor.Status != IdentityAccountStatus.Active)
            throw new ForbiddenException(HrmErrorCodes.Forbidden, "Tài khoản không hiệu lực.");
    }
}
