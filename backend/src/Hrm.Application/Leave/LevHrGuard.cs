using Hrm.Application.Common;
using Hrm.Domain.Identity.Repositories;
using Hrm.Domain.Shared.Constants;
using Jarvis.Domain.Shared.ExceptionHandling;

namespace Hrm.Application.Leave;

public static class LevHrGuard
{
    public static void RequireHrForC2(IdentityAccountSnapshot? actor)
    {
        if (actor is null || actor.Status != Domain.Identity.IdentityAccountStatus.Active)
            throw new ForbiddenException(HrmErrorCodes.Forbidden, "Tài khoản không hiệu lực.");

        if (!IamAccessGuard.IsHrOrPgd(actor))
        {
            throw new ForbiddenException(
                HrmErrorCodes.Forbidden,
                "Chỉ HR/PGD duyệt C2 (LEV-FR-012 / LEV-TC-017).");
        }
    }
}
