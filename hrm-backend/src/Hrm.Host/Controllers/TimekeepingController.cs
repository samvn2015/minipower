using Hrm.Application.Timekeeping.Commands;
using Hrm.Application.Timekeeping.Dtos;
using Hrm.Application.Timekeeping.Queries;
using Hrm.Domain.Timekeeping.Repositories;
using Hrm.Host.Extensions;
using Jarvis.Application.Contracts.Commands;
using Jarvis.Application.Contracts.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hrm.Host.Controllers;

/// <summary>TIM — DOC-12 · template master (FR-001/002/015).</summary>
[ApiController]
[Route("v1/tim")]
[Authorize]
public sealed class TimekeepingController(
    IAsyncQueryDispatcher queries,
    IAsyncCommandDispatcher commands) : ControllerBase
{
    [HttpGet("templates/active")]
    public async Task<IActionResult> GetActive(CancellationToken cancellationToken)
    {
        var dto = await queries.DispatchAsync<GetActiveTimesheetTemplateQuery, TimesheetTemplateDto?>(
            new GetActiveTimesheetTemplateQuery(),
            cancellationToken);
        return dto is null ? NotFound(new { message = "Chưa có mẫu Active." }) : Ok(dto);
    }

    [HttpGet("templates")]
    public async Task<IActionResult> List(CancellationToken cancellationToken)
    {
        var items = await queries.DispatchAsync<ListTimesheetTemplatesQuery, IReadOnlyList<TimesheetTemplateDto>>(
            new ListTimesheetTemplatesQuery(User.GetIdpSubject()),
            cancellationToken);
        return Ok(items);
    }

    [HttpPost("templates")]
    public async Task<IActionResult> Create(
        [FromBody] CreateTimesheetTemplateRequest body,
        CancellationToken cancellationToken)
    {
        var columns = body.Columns
            .Select(c => new TimesheetTemplateColumnCreateModel(
                c.ColumnKey,
                c.DisplayName,
                c.SortOrder,
                c.IsRequired,
                c.MapsTo))
            .ToList();

        var result = await commands.DispatchAsync<CreateTimesheetTemplateCommand, TimesheetTemplateCreateResult>(
            new CreateTimesheetTemplateCommand(User.GetIdpSubject(), body.VersionCode, body.Name, columns),
            cancellationToken);
        return CreatedAtAction(nameof(GetActive), result);
    }

    [HttpPost("templates/{id:guid}/publish")]
    public async Task<IActionResult> Publish(Guid id, CancellationToken cancellationToken)
    {
        var result = await commands.DispatchAsync<PublishTimesheetTemplateCommand, TimesheetTemplatePublishResult>(
            new PublishTimesheetTemplateCommand(User.GetIdpSubject(), id),
            cancellationToken);
        return Ok(result);
    }

    public sealed record CreateTimesheetTemplateRequest(
        string VersionCode,
        string Name,
        IReadOnlyList<CreateColumnRequest> Columns);

    public sealed record CreateColumnRequest(
        string ColumnKey,
        string DisplayName,
        int SortOrder,
        bool IsRequired,
        string MapsTo);
}
