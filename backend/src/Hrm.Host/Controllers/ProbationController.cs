using Hrm.Application.Probation.Commands;
using Hrm.Application.Probation.Dtos;
using Hrm.Application.Probation.Queries;
using Hrm.Host.Extensions;
using Jarvis.Application.Contracts.Commands;
using Jarvis.Application.Contracts.Queries;
using Microsoft.AspNetCore.Authorization;
using System.Globalization;
using Hrm.Domain.Shared.Paging;
using Microsoft.AspNetCore.Mvc;

namespace Hrm.Host.Controllers;

/// <summary>PRB — mốc EMP · T-15/T-7 · đề xuất/chốt SoT.</summary>
[ApiController]
[Route("v1/prb")]
[Authorize]
public sealed class ProbationController(
    IAsyncQueryDispatcher queries,
    IAsyncCommandDispatcher commands) : ControllerBase
{
    [HttpGet("cases")]
    public async Task<IActionResult> ListCases(CancellationToken cancellationToken)
    {
        var items = await queries.DispatchAsync<ListProbationCasesQuery, IReadOnlyList<ProbationCaseDto>>(
            new ListProbationCasesQuery(User.RequireIdpSubject()),
            cancellationToken);
        return Ok(items);
    }

    [HttpGet("milestones/me")]
    public async Task<IActionResult> GetMyMilestones(CancellationToken cancellationToken)
    {
        var dto = await queries.DispatchAsync<GetMyProbationMilestonesQuery, ProbationMilestoneDto>(
            new GetMyProbationMilestonesQuery(User.RequireIdpSubject()),
            cancellationToken);
        return Ok(dto);
    }

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
            new RunProbationRemindersCommand(User.RequireIdpSubject(), asOf),
            cancellationToken);
        return Ok(result);
    }

    /// <summary>S1 — body vẫn là mảng, tổng qua <c>X-Total-Count</c> (tổng khớp bộ lọc <c>kind</c>).</summary>
    [HttpGet("reminders")]
    public async Task<IActionResult> ListReminders(
        [FromQuery] string? kind,
        CancellationToken cancellationToken,
        [FromQuery] int? page = null,
        [FromQuery] int? size = null)
    {
        var result = await queries.DispatchAsync<ListProbationRemindersQuery, PagedResult<ProbationReminderDto>>(
            new ListProbationRemindersQuery(User.RequireIdpSubject(), kind, page, size),
            cancellationToken);

        Response.Headers["X-Total-Count"] = result.Total.ToString(CultureInfo.InvariantCulture);
        return Ok(result.Items);
    }

    [HttpGet("masters/outcomes")]
    public async Task<IActionResult> ListOutcomes(CancellationToken cancellationToken)
    {
        var items = await queries.DispatchAsync<ListProbationOutcomesQuery, IReadOnlyList<ProbationMasterItemDto>>(
            new ListProbationOutcomesQuery(User.RequireIdpSubject()),
            cancellationToken);
        return Ok(items);
    }

    [HttpGet("masters/criteria")]
    public async Task<IActionResult> ListCriteria(CancellationToken cancellationToken)
    {
        var items = await queries.DispatchAsync<ListProbationCriteriaQuery, IReadOnlyList<ProbationMasterItemDto>>(
            new ListProbationCriteriaQuery(User.RequireIdpSubject()),
            cancellationToken);
        return Ok(items);
    }

    [HttpGet("masters/extend-durations")]
    public async Task<IActionResult> ListExtendDurations(CancellationToken cancellationToken)
    {
        var items = await queries.DispatchAsync<
            ListProbationExtendDurationsQuery,
            IReadOnlyList<ProbationExtendDurationDto>>(
            new ListProbationExtendDurationsQuery(User.RequireIdpSubject()),
            cancellationToken);
        return Ok(items);
    }

    [HttpGet("evaluations")]
    public async Task<IActionResult> ListEvaluations(
        CancellationToken cancellationToken,
        [FromQuery] int? page = null,
        [FromQuery] int? size = null)
    {
        var result = await queries.DispatchAsync<ListProbationEvaluationsQuery, PagedResult<ProbationEvaluationDto>>(
            new ListProbationEvaluationsQuery(User.RequireIdpSubject(), page, size),
            cancellationToken);

        Response.Headers["X-Total-Count"] = result.Total.ToString(CultureInfo.InvariantCulture);
        return Ok(result.Items);
    }

    [HttpPost("evaluations/{employeeId:guid}/propose")]
    public async Task<IActionResult> Propose(
        Guid employeeId,
        [FromBody] ProposeProbationEvaluationRequest body,
        CancellationToken cancellationToken)
    {
        var scores = body.Scores?.Select(s => new ProbationCriterionScoreInput(s.CriterionCode, s.Comment)).ToList();
        var dto = await commands.DispatchAsync<ProposeProbationEvaluationCommand, ProbationEvaluationDto>(
            new ProposeProbationEvaluationCommand(
                User.RequireIdpSubject(),
                employeeId,
                body.OutcomeCode,
                body.Note,
                scores),
            cancellationToken);
        return Ok(dto);
    }

    [HttpPost("evaluations/{employeeId:guid}/decide")]
    public async Task<IActionResult> Decide(
        Guid employeeId,
        [FromBody] DecideProbationEvaluationRequest body,
        CancellationToken cancellationToken)
    {
        var scores = body.Scores?.Select(s => new ProbationCriterionScoreInput(s.CriterionCode, s.Comment)).ToList();
        var dto = await commands.DispatchAsync<DecideProbationEvaluationCommand, ProbationEvaluationDto>(
            new DecideProbationEvaluationCommand(
                User.RequireIdpSubject(),
                employeeId,
                body.OutcomeCode,
                body.Note,
                body.ExtendDurationCode,
                scores),
            cancellationToken);
        return Ok(dto);
    }
}

public sealed record RunProbationRemindersRequest(string? AsOfDate);

public sealed record ProbationCriterionScoreRequest(string CriterionCode, string? Comment);

public sealed record ProposeProbationEvaluationRequest(
    string OutcomeCode,
    string? Note,
    IReadOnlyList<ProbationCriterionScoreRequest>? Scores);

public sealed record DecideProbationEvaluationRequest(
    string OutcomeCode,
    string? Note,
    string? ExtendDurationCode,
    IReadOnlyList<ProbationCriterionScoreRequest>? Scores);
