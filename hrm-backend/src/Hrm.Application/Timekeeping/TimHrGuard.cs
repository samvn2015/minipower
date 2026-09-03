using Hrm.Application.Common;
using Hrm.Domain.Identity.Constants;
using Hrm.Domain.Identity.Repositories;
using Hrm.Domain.Shared.Constants;
using Jarvis.Domain.Shared.ExceptionHandling;

namespace Hrm.Application.Timekeeping;

public static class TimHrGuard
{
    public static void RequireHrOrItForTemplate(IdentityAccountSnapshot? actor)
    {
        if (!IamAccessGuard.IsHrOrIt(actor))
        {
            throw new ForbiddenException(
                HrmErrorCodes.Forbidden,
                "Chỉ HR/IT công bố hoặc quản lý mẫu TIM (TIM-FR-011 / TIM-BR-010).");
        }
    }

    public static void RequireHrForImport(IdentityAccountSnapshot? actor)
    {
        if (actor is null || actor.Status != Domain.Identity.IdentityAccountStatus.Active)
            throw new ForbiddenException(HrmErrorCodes.Forbidden, "Tài khoản không hiệu lực.");

        if (!actor.RoleCodes.Any(static r =>
                string.Equals(r, IamRoleCodes.Hr, StringComparison.OrdinalIgnoreCase)))
        {
            throw new ForbiddenException(
                HrmErrorCodes.Forbidden,
                "Chỉ HR import/commit/chốt bảng công (TIM-FR-011 / TIM-BR-010).");
        }
    }
}
