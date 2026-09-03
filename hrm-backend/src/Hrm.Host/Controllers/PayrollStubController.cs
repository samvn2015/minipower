using Hrm.Application.Common;
using Hrm.Domain.Identity.Repositories;
using Hrm.Domain.Payroll.Repositories;
using Hrm.Domain.Shared.Constants;
using Hrm.Host.Extensions;
using Jarvis.Domain.Shared.ExceptionHandling;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hrm.Host.Controllers;

/// <summary>PAY stub — chỉ cổng kỳ chốt để TIM-FR-012. Tính lương full = slice sau.</summary>
[ApiController]
[Route("v1/pay")]
[Authorize]
public sealed class PayrollStubController(IIdentityAccountReadRepository accounts, IPayPeriodRepository payPeriods)
    : ControllerBase
{
    /// <summary>Đánh dấu kỳ PAY Closed (stub gate). Không tính phiếu.</summary>
    [HttpPost("periods/{ym}/close")]
    public async Task<IActionResult> MarkClosed(string ym, CancellationToken cancellationToken)
    {
        var subject = User.GetIdpSubject();
        IamAccessGuard.RequireAuthenticated(subject);
        var actor = await accounts.FindByIdpSubjectAsync(subject!, cancellationToken).ConfigureAwait(false);
        if (actor is null
            || !actor.RoleCodes.Any(static r =>
                string.Equals(r, "IAM-ROLE-HR", StringComparison.OrdinalIgnoreCase)))
        {
            throw new ForbiddenException(HrmErrorCodes.Forbidden, "Chỉ HR đánh dấu kỳ PAY Closed (stub).");
        }

        if (string.IsNullOrWhiteSpace(ym) || ym.Length != 7 || ym[4] != '-')
            return BadRequest(new { message = "PeriodYm phải dạng YYYY-MM." });

        await payPeriods.MarkClosedAsync(ym.Trim(), subject!, cancellationToken).ConfigureAwait(false);
        return Ok(new { periodYm = ym.Trim(), status = "Closed" });
    }
}
