using Hrm.Application.Probation.Dtos;
using Hrm.Application.Probation.Queries;
using Hrm.Host.Extensions;
using Jarvis.Application.Contracts.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hrm.Host.Controllers;

/// <summary>PRB — mốc TV từ EMP (FR-001) · hàng TV (FR-008) · SCR-004 (FR-015).</summary>
[ApiController]
[Route("v1/prb")]
[Authorize]
public sealed class ProbationController(IAsyncQueryDispatcher queries) : ControllerBase
{
    /// <summary>PRB-SCR-001 — HR/PGD: mọi NV đang TV (không bịa KT).</summary>
    [HttpGet("cases")]
    public async Task<IActionResult> ListCases(CancellationToken cancellationToken)
    {
        var items = await queries.DispatchAsync<ListProbationCasesQuery, IReadOnlyList<ProbationCaseDto>>(
            new ListProbationCasesQuery(User.GetIdpSubject()),
            cancellationToken);
        return Ok(items);
    }

    /// <summary>PRB-SCR-004 — NV: mốc BĐ/KT chỉ từ HĐ EMP; không date picker ảo.</summary>
    [HttpGet("milestones/me")]
    public async Task<IActionResult> GetMyMilestones(CancellationToken cancellationToken)
    {
        var dto = await queries.DispatchAsync<GetMyProbationMilestonesQuery, ProbationMilestoneDto>(
            new GetMyProbationMilestonesQuery(User.GetIdpSubject()),
            cancellationToken);
        return Ok(dto);
    }
}
