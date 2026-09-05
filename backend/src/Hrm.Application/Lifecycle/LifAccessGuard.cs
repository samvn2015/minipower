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

    /// <summary>Khóa Git/CRM SP — chỉ IT/PGD; HR 403 (LIF-FR-008).</summary>
    public static void RequireItOrPgdForLocks(IdentityAccountSnapshot? actor)
    {
        if (actor is null || actor.Status != IdentityAccountStatus.Active)
            throw new ForbiddenException(HrmErrorCodes.Forbidden, "Không có quyền LIF.");

        if (actor.RoleCodes.Any(static r =>
                string.Equals(r, IamRoleCodes.Hr, StringComparison.OrdinalIgnoreCase))
            && !actor.RoleCodes.Any(static r =>
                string.Equals(r, IamRoleCodes.It, StringComparison.OrdinalIgnoreCase)
                || string.Equals(r, IamRoleCodes.Pgd, StringComparison.OrdinalIgnoreCase)))
        {
            throw new ForbiddenException(
                HrmErrorCodes.Forbidden,
                "HR không khóa Git/CRM SP trực tiếp (LIF-FR-008) — chỉ xem + ticket IT.");
        }

        if (!actor.RoleCodes.Any(static r =>
                string.Equals(r, IamRoleCodes.It, StringComparison.OrdinalIgnoreCase)
                || string.Equals(r, IamRoleCodes.Pgd, StringComparison.OrdinalIgnoreCase)))
        {
            throw new ForbiddenException(
                HrmErrorCodes.Forbidden,
                "Chỉ IT/PGD (job) khóa Git + CRM SP (LIF-FR-005/008).");
        }
    }

    public static void RequireHrItOrPgd(IdentityAccountSnapshot? actor)
    {
        if (actor is null || actor.Status != IdentityAccountStatus.Active)
            throw new ForbiddenException(HrmErrorCodes.Forbidden, "Không có quyền LIF.");

        if (!actor.RoleCodes.Any(static r =>
                string.Equals(r, IamRoleCodes.Hr, StringComparison.OrdinalIgnoreCase)
                || string.Equals(r, IamRoleCodes.It, StringComparison.OrdinalIgnoreCase)
                || string.Equals(r, IamRoleCodes.Pgd, StringComparison.OrdinalIgnoreCase)))
        {
            throw new ForbiddenException(
                HrmErrorCodes.Forbidden,
                "Chỉ HR/IT/PGD xem LIF và cấp TK / khóa hệ thống (LIF-FR-002/005/008).");
        }
    }

    public static void RequireAuthenticated(IdentityAccountSnapshot? actor)
    {
        if (actor is null || actor.Status != IdentityAccountStatus.Active)
            throw new ForbiddenException(HrmErrorCodes.Forbidden, "Tài khoản không hiệu lực.");
    }
}
