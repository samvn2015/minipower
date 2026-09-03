using Hrm.Domain.Identity.Constants;
using Hrm.Domain.Identity.Repositories;
using Hrm.Domain.Shared.Constants;
using Jarvis.Domain.Shared.ExceptionHandling;

namespace Hrm.Application.Payroll;

/// <summary>Cô lập phiếu lương — PAY-FR-010 · BR-007.</summary>
public static class PayPayslipAccess
{
    public static bool IsHr(IdentityAccountSnapshot actor) =>
        actor.RoleCodes.Any(static r =>
            string.Equals(r, IamRoleCodes.Hr, StringComparison.OrdinalIgnoreCase));

    public static void EnsureCanView(IdentityAccountSnapshot actor, string payslipEmployeeCode)
    {
        if (actor.Status != Domain.Identity.IdentityAccountStatus.Active)
            throw new ForbiddenException(HrmErrorCodes.Forbidden, "Tài khoản không hiệu lực.");

        if (IsHr(actor))
            return;

        if (!string.IsNullOrWhiteSpace(actor.EmployeeCode)
            && string.Equals(actor.EmployeeCode, payslipEmployeeCode, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        throw new ForbiddenException(
            HrmErrorCodes.Forbidden,
            "Không xem phiếu lương người khác (PAY-FR-010).");
    }
}
