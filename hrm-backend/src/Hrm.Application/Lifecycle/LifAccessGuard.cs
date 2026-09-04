using Hrm.Domain.Identity;
using Hrm.Domain.Identity.Constants;
using Hrm.Domain.Identity.Repositories;
using Hrm.Domain.Shared.Constants;
using Jarvis.Domain.Shared.ExceptionHandling;

namespace Hrm.Application.Lifecycle;

public static class LifAccessGuard
{
    public static void RequireHrOrPgd(IdentityAccountSnapshot? actor)
    {
        if (actor is null || actor.Status != IdentityAccountStatus.Active)
            throw new ForbiddenException(HrmErrorCodes.Forbidden, "Không có quyền LIF.");

        if (!actor.RoleCodes.Any(static r =>
                string.Equals(r, IamRoleCodes.Hr, StringComparison.OrdinalIgnoreCase)
                || string.Equals(r, IamRoleCodes.Pgd, StringComparison.OrdinalIgnoreCase)))
        {
            throw new ForbiddenException(
                HrmErrorCodes.Forbidden,
                "Chỉ HR/PGD thao tác offboarding (LIF-FR-003/015).");
        }
    }

    public static void RequireAuthenticated(IdentityAccountSnapshot? actor)
    {
        if (actor is null || actor.Status != IdentityAccountStatus.Active)
            throw new ForbiddenException(HrmErrorCodes.Forbidden, "Tài khoản không hiệu lực.");
    }
}
