using Hrm.Application.Common;
using Hrm.Domain.Identity.Constants;
using Hrm.Domain.Identity.Repositories;
using Hrm.Domain.Shared.Constants;
using Jarvis.Domain.Shared.ExceptionHandling;

namespace Hrm.Application.Payroll;

public static class PayHrGuard
{
    public static void RequireHr(IdentityAccountSnapshot? actor)
    {
        if (actor is null || actor.Status != Domain.Identity.IdentityAccountStatus.Active)
            throw new ForbiddenException(HrmErrorCodes.Forbidden, "Tài khoản không hiệu lực.");

        if (!actor.RoleCodes.Any(static r =>
                string.Equals(r, IamRoleCodes.Hr, StringComparison.OrdinalIgnoreCase)))
        {
            throw new ForbiddenException(
                HrmErrorCodes.Forbidden,
                "Chỉ HR tính/chốt kỳ lương (PAY-FR-001).");
        }
    }
}
