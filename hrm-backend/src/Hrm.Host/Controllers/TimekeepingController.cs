using Hrm.Application.Timekeeping.Commands;
using Hrm.Application.Timekeeping.Dtos;
using Hrm.Application.Timekeeping.Queries;
using Hrm.Application.Timekeeping;
using Hrm.Domain.Timekeeping.Repositories;
using Hrm.Host.Extensions;
using Jarvis.Application.Contracts.Commands;
using Jarvis.Application.Contracts.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hrm.Host.Controllers;

/// <summary>TIM — template master + import preview/commit (FR-001…005).</summary>
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

    /// <summary>Preview import — JSON rows (CSV/Excel parse client-side MVP / e2e).</summary>
    [HttpPost("imports")]
    public async Task<IActionResult> PreviewImport(
        [FromBody] PreviewImportRequest body,
        CancellationToken cancellationToken)
    {
        var rows = body.Rows.Select((r, i) => new TimesheetImportRowValidator.RawImportRow(
            r.RowNumber > 0 ? r.RowNumber : i + 1,
            r.EmployeeCode,
            r.WorkDays,
            r.Ot15,
            r.Ot20,
            r.Ot30)).ToList();

        var result = await commands.DispatchAsync<PreviewTimesheetImportCommand, TimesheetImportBatchDto>(
            new PreviewTimesheetImportCommand(
                User.GetIdpSubject(),
                body.PeriodYm,
                body.TemplateVersionCode,
                body.FileName,
                rows),
            cancellationToken);
        return Ok(result);
    }

    [HttpGet("imports/{id:guid}")]
    public async Task<IActionResult> GetImport(Guid id, CancellationToken cancellationToken)
    {
        var dto = await queries.DispatchAsync<GetTimesheetImportBatchQuery, TimesheetImportBatchDto>(
            new GetTimesheetImportBatchQuery(User.GetIdpSubject(), id),
            cancellationToken);
        return Ok(dto);
    }

    [HttpPost("imports/{id:guid}/commit")]
    public async Task<IActionResult> CommitImport(Guid id, CancellationToken cancellationToken)
    {
        var result = await commands.DispatchAsync<CommitTimesheetImportCommand, TimesheetCommitResult>(
            new CommitTimesheetImportCommand(User.GetIdpSubject(), id),
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

    public sealed record PreviewImportRequest(
        string PeriodYm,
        string TemplateVersionCode,
        string? FileName,
        IReadOnlyList<PreviewImportRowRequest> Rows);

    public sealed record PreviewImportRowRequest(
        int RowNumber,
        string? EmployeeCode,
        decimal? WorkDays,
        decimal? Ot15,
        decimal? Ot20,
        decimal? Ot30);
}
