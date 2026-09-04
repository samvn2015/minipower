using Hrm.Application.Lifecycle.Commands;
using Hrm.Application.Lifecycle.Dtos;
using Hrm.Application.Lifecycle.Queries;
using Hrm.Host.Extensions;
using Jarvis.Application.Contracts.Commands;
using Jarvis.Application.Contracts.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hrm.Host.Controllers;

/// <summary>LIF — offboarding N / N+3 (FR-003/004/013/015).</summary>
[ApiController]
[Route("v1/lif")]
[Authorize]
public sealed class LifecycleController(
    IAsyncQueryDispatcher queries,
    IAsyncCommandDispatcher commands) : ControllerBase
{
    /// <summary>LIF-SCR-001 — danh sách case (Open + ConfirmedN).</summary>
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

    /// <summary>LIF-SCR-003 — HR xác nhận N = ngày LV cuối (không phải ngày ký).</summary>
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
}

public sealed record CreateLifOffboardingRequest(
    Guid EmployeeId,
    DateOnly? ResignationSignedDate,
    string? Note);

public sealed record ConfirmLifOffboardingNRequest(DateOnly LastWorkingDayN);
