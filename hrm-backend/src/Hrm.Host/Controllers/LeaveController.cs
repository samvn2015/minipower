using Hrm.Application.Leave.Commands;
using Hrm.Application.Leave.Dtos;
using Hrm.Application.Leave.Queries;
using Hrm.Domain.Leave;
using Hrm.Host.Extensions;
using Jarvis.Application.Contracts.Commands;
using Jarvis.Application.Contracts.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hrm.Host.Controllers;

/// <summary>LEV — DOC-12 · balance · submit · C1 · C2 (FR-012).</summary>
[ApiController]
[Route("v1/lev")]
[Authorize]
public sealed class LeaveController(
    IAsyncQueryDispatcher queries,
    IAsyncCommandDispatcher commands) : ControllerBase
{
    [HttpGet("leave-types")]
    public async Task<IActionResult> ListLeaveTypes(CancellationToken cancellationToken)
    {
        var items = await queries.DispatchAsync<ListLeaveTypesQuery, IReadOnlyList<LeaveTypeDto>>(
            new ListLeaveTypesQuery(),
            cancellationToken);
        return Ok(items);
    }

    [HttpGet("leave-balances/me")]
    public async Task<IActionResult> GetMyBalance(
        [FromQuery] int? year,
        CancellationToken cancellationToken)
    {
        var dto = await queries.DispatchAsync<GetMyLeaveBalanceQuery, LeaveBalanceDto>(
            new GetMyLeaveBalanceQuery(User.GetIdpSubject(), year),
            cancellationToken);
        return Ok(dto);
    }

    [HttpGet("leave-requests/me")]
    public async Task<IActionResult> ListMyRequests(CancellationToken cancellationToken)
    {
        var items = await queries.DispatchAsync<ListMyLeaveRequestsQuery, IReadOnlyList<LeaveRequestDto>>(
            new ListMyLeaveRequestsQuery(User.GetIdpSubject()),
            cancellationToken);
        return Ok(items);
    }

    [HttpPost("leave-requests")]
    public async Task<IActionResult> Create(
        [FromBody] CreateLeaveRequestRequest body,
        CancellationToken cancellationToken)
    {
        if (!Enum.TryParse<LeaveDayPart>(body.DayPart, ignoreCase: true, out var dayPart))
            return BadRequest(new { message = "DayPart không hợp lệ (FullDay/Morning/Afternoon)." });

        var result = await commands.DispatchAsync<CreateLeaveRequestCommand, LeaveRequestCreateResult>(
            new CreateLeaveRequestCommand(
                User.GetIdpSubject(),
                body.LeaveTypeCode,
                body.FromDate,
                body.ToDate,
                dayPart,
                body.Reason,
                body.HandoverEmployeeId,
                body.IsEmergency),
            cancellationToken);
        return CreatedAtAction(nameof(ListMyRequests), result);
    }

    [HttpGet("leave-requests/pending-c1")]
    public async Task<IActionResult> ListPendingC1(CancellationToken cancellationToken)
    {
        var items = await queries.DispatchAsync<
            ListPendingLeaveRequestsC1Query,
            IReadOnlyList<LeaveRequestPendingC1Dto>>(
            new ListPendingLeaveRequestsC1Query(User.GetIdpSubject()),
            cancellationToken);
        return Ok(items);
    }

    [HttpPost("leave-requests/{id:guid}/c1/approve")]
    public async Task<IActionResult> ApproveC1(Guid id, CancellationToken cancellationToken)
    {
        var result = await commands.DispatchAsync<ApproveLeaveRequestC1Command, LeaveRequestActionResult>(
            new ApproveLeaveRequestC1Command(User.GetIdpSubject(), id),
            cancellationToken);
        return Ok(result);
    }

    [HttpPost("leave-requests/{id:guid}/c1/reject")]
    public async Task<IActionResult> RejectC1(
        Guid id,
        [FromBody] RejectLeaveRequestC1Request? body,
        CancellationToken cancellationToken)
    {
        var result = await commands.DispatchAsync<RejectLeaveRequestC1Command, LeaveRequestActionResult>(
            new RejectLeaveRequestC1Command(User.GetIdpSubject(), id, body?.ReviewNote),
            cancellationToken);
        return Ok(result);
    }

    [HttpGet("leave-requests/pending-c2")]
    public async Task<IActionResult> ListPendingC2(CancellationToken cancellationToken)
    {
        var items = await queries.DispatchAsync<
            ListPendingLeaveRequestsC2Query,
            IReadOnlyList<LeaveRequestPendingC1Dto>>(
            new ListPendingLeaveRequestsC2Query(User.GetIdpSubject()),
            cancellationToken);
        return Ok(items);
    }

    [HttpPost("leave-requests/{id:guid}/c2/approve")]
    public async Task<IActionResult> ApproveC2(Guid id, CancellationToken cancellationToken)
    {
        var result = await commands.DispatchAsync<ApproveLeaveRequestC2Command, LeaveRequestActionResult>(
            new ApproveLeaveRequestC2Command(User.GetIdpSubject(), id),
            cancellationToken);
        return Ok(result);
    }

    [HttpPost("leave-requests/{id:guid}/c2/reject")]
    public async Task<IActionResult> RejectC2(
        Guid id,
        [FromBody] RejectLeaveRequestC2Request? body,
        CancellationToken cancellationToken)
    {
        var result = await commands.DispatchAsync<RejectLeaveRequestC2Command, LeaveRequestActionResult>(
            new RejectLeaveRequestC2Command(User.GetIdpSubject(), id, body?.ReviewNote),
            cancellationToken);
        return Ok(result);
    }

    public sealed record RejectLeaveRequestC2Request(string? ReviewNote);

    public sealed record RejectLeaveRequestC1Request(string? ReviewNote);

    public sealed record CreateLeaveRequestRequest(
        string LeaveTypeCode,
        DateOnly FromDate,
        DateOnly ToDate,
        string DayPart,
        string Reason,
        Guid HandoverEmployeeId,
        bool IsEmergency = false);
}
