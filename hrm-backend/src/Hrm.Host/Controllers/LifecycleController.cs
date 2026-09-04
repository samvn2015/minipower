using Hrm.Application.Lifecycle.Commands;
using Hrm.Application.Lifecycle.Dtos;
using Hrm.Application.Lifecycle.Queries;
using Hrm.Host.Extensions;
using Jarvis.Application.Contracts.Commands;
using Jarvis.Application.Contracts.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hrm.Host.Controllers;

/// <summary>LIF — offboarding N/N+3, checklist, khóa Git+CRM SP (FR-003/005–010/009).</summary>
[ApiController]
[Route("v1/lif")]
[Authorize]
public sealed class LifecycleController(
    IAsyncQueryDispatcher queries,
    IAsyncCommandDispatcher commands) : ControllerBase
{
    [HttpGet("offboarding")]
    public async Task<IActionResult> List(CancellationToken cancellationToken)
    {
        var items = await queries.DispatchAsync<ListLifOffboardingQuery, IReadOnlyList<LifOffboardingDto>>(
            new ListLifOffboardingQuery(User.GetIdpSubject()),
            cancellationToken);
        return Ok(items);
    }

    [HttpGet("offboarding/open")]
    public Task<IActionResult> ListOpen(CancellationToken cancellationToken) =>
        List(cancellationToken);

    [HttpGet("offboarding/{id:guid}")]
    public async Task<IActionResult> Get(Guid id, CancellationToken cancellationToken)
    {
        var dto = await queries.DispatchAsync<GetLifOffboardingQuery, LifOffboardingDto>(
            new GetLifOffboardingQuery(User.GetIdpSubject(), id),
            cancellationToken);
        return Ok(dto);
    }

    [HttpPost("offboarding")]
    public async Task<IActionResult> Create(
        [FromBody] CreateLifOffboardingRequest body,
        CancellationToken cancellationToken)
    {
        var dto = await commands.DispatchAsync<CreateLifOffboardingCommand, LifOffboardingDto>(
            new CreateLifOffboardingCommand(
                User.GetIdpSubject(),
                body.EmployeeId,
                body.ResignationSignedDate,
                body.Note),
            cancellationToken);
        return CreatedAtAction(nameof(Get), new { id = dto.Id }, dto);
    }

    [HttpPost("offboarding/{id:guid}/confirm-n")]
    public async Task<IActionResult> ConfirmN(
        Guid id,
        [FromBody] ConfirmLifOffboardingNRequest body,
        CancellationToken cancellationToken)
    {
        var dto = await commands.DispatchAsync<ConfirmLifOffboardingNCommand, LifOffboardingDto>(
            new ConfirmLifOffboardingNCommand(User.GetIdpSubject(), id, body.LastWorkingDayN),
            cancellationToken);
        return Ok(dto);
    }

    /// <summary>LIF-SCR-005 — checklist off từ master.</summary>
    [HttpGet("offboarding/{id:guid}/checklist")]
    public async Task<IActionResult> GetChecklist(Guid id, CancellationToken cancellationToken)
    {
        var dto = await queries.DispatchAsync<GetLifOffChecklistQuery, LifOffChecklistBoardDto>(
            new GetLifOffChecklistQuery(User.GetIdpSubject(), id),
            cancellationToken);
        return Ok(dto);
    }

    [HttpPut("offboarding/{id:guid}/checklist/{itemCode}")]
    public async Task<IActionResult> UpsertTick(
        Guid id,
        string itemCode,
        [FromBody] UpsertLifOffChecklistTickRequest body,
        CancellationToken cancellationToken)
    {
        var dto = await commands.DispatchAsync<UpsertLifOffChecklistTickCommand, LifOffChecklistBoardDto>(
            new UpsertLifOffChecklistTickCommand(User.GetIdpSubject(), id, itemCode, body.IsChecked),
            cancellationToken);
        return Ok(dto);
    }

    [HttpPost("offboarding/{id:guid}/close")]
    public async Task<IActionResult> Close(Guid id, CancellationToken cancellationToken)
    {
        var dto = await commands.DispatchAsync<CloseLifOffboardingCommand, LifOffboardingDto>(
            new CloseLifOffboardingCommand(User.GetIdpSubject(), id),
            cancellationToken);
        return Ok(dto);
    }

    /// <summary>IT khóa Git+CRM SP một case — HR 403 (LIF-FR-005…008/014).</summary>
    [HttpPost("offboarding/{id:guid}/locks")]
    public async Task<IActionResult> ApplyLocks(
        Guid id,
        [FromBody] ApplyLifOffboardingLocksRequest? body,
        CancellationToken cancellationToken)
    {
        DateOnly? asOf = null;
        if (!string.IsNullOrWhiteSpace(body?.AsOfDate)
            && DateOnly.TryParse(body.AsOfDate, out var d))
            asOf = d;

        var dto = await commands.DispatchAsync<ApplyLifOffboardingLocksCommand, LifOffboardingDto>(
            new ApplyLifOffboardingLocksCommand(
                User.GetIdpSubject(),
                id,
                asOf,
                body?.EarlyCrReason),
            cancellationToken);
        return Ok(dto);
    }

    /// <summary>Job N+3 — khóa hàng loạt khi ≥ N+3; không early không CR.</summary>
    [HttpPost("offboarding/jobs/nplus3-locks")]
    public async Task<IActionResult> RunNPlus3Locks(
        [FromBody] RunLifNPlus3LocksRequest? body,
        CancellationToken cancellationToken)
    {
        DateOnly? asOf = null;
        if (!string.IsNullOrWhiteSpace(body?.AsOfDate)
            && DateOnly.TryParse(body.AsOfDate, out var d))
            asOf = d;

        var result = await commands.DispatchAsync<RunLifNPlus3LocksCommand, LifNPlus3LockRunResult>(
            new RunLifNPlus3LocksCommand(User.GetIdpSubject(), asOf),
            cancellationToken);
        return Ok(result);
    }
}

public sealed record CreateLifOffboardingRequest(
    Guid EmployeeId,
    DateOnly? ResignationSignedDate,
    string? Note);

public sealed record ConfirmLifOffboardingNRequest(DateOnly LastWorkingDayN);

public sealed record UpsertLifOffChecklistTickRequest(bool IsChecked);

public sealed record ApplyLifOffboardingLocksRequest(string? AsOfDate, string? EarlyCrReason);

public sealed record RunLifNPlus3LocksRequest(string? AsOfDate);
