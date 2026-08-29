using Hrm.Domain.Shared.Constants;
using Hrm.Domain.Identity.Constants;
using Hrm.Domain.Identity.Repositories;
using Jarvis.Domain.Shared.ExceptionHandling;

namespace Hrm.Application.Common;

public static class IamAccessGuard
{
    public static void RequireAuthenticated(string? actorIdpSubject)
    {
        if (string.IsNullOrWhiteSpace(actorIdpSubject))
            throw new UnauthorizedException(HrmErrorCodes.Unauthorized, "Thiếu claim sub.");
    }

    public static void RequireHrOrIt(IdentityAccountSnapshot? actor)
    {
        if (actor is null || actor.Status != Domain.Identity.IdentityAccountStatus.Active)
            throw new ForbiddenException(HrmErrorCodes.Forbidden, "Không có quyền IAM admin.");

        if (!actor.RoleCodes.Any(static r =>
                string.Equals(r, IamRoleCodes.Hr, StringComparison.OrdinalIgnoreCase)
                || string.Equals(r, IamRoleCodes.It, StringComparison.OrdinalIgnoreCase)))
        {
            throw new ForbiddenException(HrmErrorCodes.Forbidden, "Chỉ HR hoặc IT được thao tác IAM-SCR-003 (IAM-FR-013).");
        }
    }

    public static void RequireIt(IdentityAccountSnapshot? actor)
    {
        if (actor is null || actor.Status != Domain.Identity.IdentityAccountStatus.Active)
            throw new ForbiddenException(HrmErrorCodes.Forbidden, "Không có quyền IAM admin.");

        if (!actor.RoleCodes.Any(static r =>
                string.Equals(r, IamRoleCodes.It, StringComparison.OrdinalIgnoreCase)))
        {
            throw new ForbiddenException(HrmErrorCodes.Forbidden, "Chỉ IT được vô hiệu login IAM-SCR-004 (IAM-FR-010).");
        }
    }

    public static bool IsHrOrIt(IdentityAccountSnapshot? actor) =>
        actor is { Status: Domain.Identity.IdentityAccountStatus.Active }
        && actor.RoleCodes.Any(static r =>
            string.Equals(r, IamRoleCodes.Hr, StringComparison.OrdinalIgnoreCase)
            || string.Equals(r, IamRoleCodes.It, StringComparison.OrdinalIgnoreCase));

    public static bool IsHrOrPgd(IdentityAccountSnapshot? actor) =>
        actor is { Status: Domain.Identity.IdentityAccountStatus.Active }
        && actor.RoleCodes.Any(static r =>
            string.Equals(r, IamRoleCodes.Hr, StringComparison.OrdinalIgnoreCase)
            || string.Equals(r, IamRoleCodes.Pgd, StringComparison.OrdinalIgnoreCase));

    public static void RequireHrOrPgd(IdentityAccountSnapshot? actor)
    {
        if (!IsHrOrPgd(actor))
            throw new ForbiddenException(HrmErrorCodes.Forbidden, "Chỉ HR/PGD duyệt đổi LM (EMP-SCR-006).");
    }
}
