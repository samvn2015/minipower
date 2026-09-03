using Hrm.Domain.Lifecycle.Repositories;
using Hrm.Domain.Identity.Repositories;
using Hrm.Application.Probation;
using Hrm.Domain.Shared.Constants;
using Jarvis.Domain.Shared.ExceptionHandling;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Hrm.Host.Extensions;

namespace Hrm.Host.Controllers;

/// <summary>LIF slice A stub — case off mở từ PRB-FAIL (PRB-FR-007).</summary>
[ApiController]
[Route("v1/lif")]
[Authorize]
public sealed class LifecycleController(
    IIdentityAccountReadRepository accounts,
    ILifOffboardingRepository offboardings) : ControllerBase
{
    [HttpGet("offboarding/open")]
    public async Task<IActionResult> ListOpen(CancellationToken cancellationToken)
    {
        var actor = await accounts.FindByIdpSubjectAsync(User.GetIdpSubject(), cancellationToken)
            ?? throw new ForbiddenException(HrmErrorCodes.Forbidden, "Tài khoản không map.");
        PrbAccessGuard.RequireHrOrPgd(actor);

        var rows = await offboardings.ListOpenAsync(cancellationToken);
        return Ok(rows.Select(r => new
        {
            r.Id,
            r.EmployeeId,
            r.EmployeeCode,
            r.Source,
            Status = r.Status.ToString(),
            r.LastWorkingDayN,
            r.CreatedAtUtc,
            r.CreatedByIdpSubject,
            r.Note
        }));
    }
}
