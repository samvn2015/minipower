using Hrm.Application.Probation.Commands;
using Hrm.Application.Probation.Dtos;
using Hrm.Application.Probation.Queries;
using Hrm.Host.Extensions;
using Jarvis.Application.Contracts.Commands;
using Jarvis.Application.Contracts.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hrm.Host.Controllers;

/// <summary>PRB — mốc EMP (FR-001) · T-15/T-7 (FR-002/003/011).</summary>
[ApiController]
[Route("v1/prb")]
[Authorize]
public sealed class ProbationController(
    IAsyncQueryDispatcher queries,
    IAsyncCommandDispatcher commands) : ControllerBase
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

    /// <summary>Chạy job nhắc T-15/T-7 theo ngày lịch (FR-002/003/008/011). Không CRM sales.</summary>
    [HttpPost("jobs/reminders/run")]
    public async Task<IActionResult> RunReminders(
        [FromBody] RunProbationRemindersRequest? body,
        CancellationToken cancellationToken)
    {
        DateOnly? asOf = null;
        if (!string.IsNullOrWhiteSpace(body?.AsOfDate))
        {
            if (!DateOnly.TryParse(body.AsOfDate, out var parsed))
                return BadRequest(new { message = "AsOfDate phải dạng yyyy-MM-dd." });
            asOf = parsed;
        }

        var result = await commands.DispatchAsync<RunProbationRemindersCommand, ProbationReminderRunResult>(
            new RunProbationRemindersCommand(User.GetIdpSubject(), asOf),
            cancellationToken);
        return Ok(result);
    }

    [HttpGet("reminders")]
    public async Task<IActionResult> ListReminders(
        [FromQuery] string? kind,
        CancellationToken cancellationToken)
    {
        var items = await queries.DispatchAsync<ListProbationRemindersQuery, IReadOnlyList<ProbationReminderDto>>(
            new ListProbationRemindersQuery(User.GetIdpSubject(), kind),
            cancellationToken);
        return Ok(items);
    }
}

public sealed record RunProbationRemindersRequest(string? AsOfDate);
