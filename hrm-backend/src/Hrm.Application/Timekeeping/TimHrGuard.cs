using Hrm.Application.Common;
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
}
