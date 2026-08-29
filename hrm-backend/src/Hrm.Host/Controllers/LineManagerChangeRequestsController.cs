using Hrm.Application.Employees.Commands;
using Hrm.Application.Employees.Dtos;
using Hrm.Application.Employees.Queries;
using Hrm.Host.Extensions;
using Jarvis.Application.Contracts.Commands;
using Jarvis.Application.Contracts.Queries;
using Jarvis.Domain.Shared.ExceptionHandling;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hrm.Host.Controllers;

/// <summary>EMP-SCR-005/006 — đổi LM có duyệt.</summary>
[ApiController]
[Route("v1/emp/line-manager-change-requests")]
[Authorize]
public sealed class LineManagerChangeRequestsController(
    IAsyncQueryDispatcher queries,
    IAsyncCommandDispatcher commands) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> ListPending(CancellationToken cancellationToken)
    {
        var items = await queries.DispatchAsync<
            ListPendingLineManagerChangesQuery,
            IReadOnlyList<LineManagerChangeDto>>(
            new ListPendingLineManagerChangesQuery(User.GetIdpSubject()),
            cancellationToken);
        return Ok(items);
    }

    [HttpPost("{id:guid}/approve")]
    public async Task<IActionResult> Approve(Guid id, CancellationToken cancellationToken)
    {
        var result = await commands.DispatchAsync<ApproveLineManagerChangeCommand, LineManagerChangeResult>(
            new ApproveLineManagerChangeCommand(User.GetIdpSubject(), id),
            cancellationToken);
        return Ok(result);
    }

    [HttpPost("{id:guid}/reject")]
    public async Task<IActionResult> Reject(
        Guid id,
        [FromBody] RejectLineManagerChangeRequest? body,
        CancellationToken cancellationToken)
    {
        var result = await commands.DispatchAsync<RejectLineManagerChangeCommand, LineManagerChangeResult>(
            new RejectLineManagerChangeCommand(User.GetIdpSubject(), id, body?.ReviewNote),
            cancellationToken);
        return Ok(result);
    }

    public sealed record RejectLineManagerChangeRequest(string? ReviewNote);
}

[ApiController]
[Route("v1/emp/employees")]
[Authorize]
public sealed class EmployeeLineManagerChangeController(IAsyncCommandDispatcher commands) : ControllerBase
{
    [HttpPost("{employeeId:guid}/line-manager-change-requests")]
    public async Task<IActionResult> Submit(
        Guid employeeId,
        [FromBody] SubmitLineManagerChangeRequest body,
        CancellationToken cancellationToken)
    {
        var result = await commands.DispatchAsync<SubmitLineManagerChangeCommand, LineManagerChangeResult>(
            new SubmitLineManagerChangeCommand(
                User.GetIdpSubject(),
                employeeId,
                body.ProposedLineManagerEmployeeId),
            cancellationToken);
        return CreatedAtRoute(
            routeName: null,
            routeValues: new { id = result.RequestId },
            value: result);
    }

    public sealed record SubmitLineManagerChangeRequest(Guid ProposedLineManagerEmployeeId);
}
