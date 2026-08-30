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

/// <summary>LEV — DOC-12 · LEV-FR-015 balance · LEV-FR-001 submit.</summary>
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

    public sealed record CreateLeaveRequestRequest(
        string LeaveTypeCode,
        DateOnly FromDate,
        DateOnly ToDate,
        string DayPart,
        string Reason,
        Guid HandoverEmployeeId,
        bool IsEmergency = false);
}
