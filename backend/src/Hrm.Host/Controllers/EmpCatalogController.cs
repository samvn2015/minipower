using Hrm.Application.Employees.Dtos;
using Hrm.Application.Employees.Queries;
using Hrm.Host.Extensions;
using Jarvis.Application.Contracts.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hrm.Host.Controllers;

[ApiController]
[Route("v1/emp")]
public sealed class EmpCatalogController(IAsyncQueryDispatcher queries) : ControllerBase
{
    [HttpGet("education-levels")]
    [Authorize]
    public async Task<IActionResult> ListEducationLevels(CancellationToken cancellationToken)
    {
        var items = await queries.DispatchAsync<ListEducationLevelsQuery, IReadOnlyList<EducationLevelDto>>(
            new ListEducationLevelsQuery(),
            cancellationToken);
        return Ok(items);
    }

    /// <summary>Master loại HĐ — EMP-FR-014 (UI lấy từ đây, không hardcode URD).</summary>
    [HttpGet("contract-types")]
    [Authorize]
    public async Task<IActionResult> ListContractTypes(CancellationToken cancellationToken)
    {
        var items = await queries.DispatchAsync<ListContractTypesQuery, IReadOnlyList<EmpCatalogItemDto>>(
            new ListContractTypesQuery(),
            cancellationToken);
        return Ok(items);
    }

    [HttpGet("employees/{employeeId:guid}/audit-logs")]
    [Authorize]
    public async Task<IActionResult> ListAuditLogs(Guid employeeId, CancellationToken cancellationToken)
    {
        var items = await queries.DispatchAsync<ListEmployeeAuditLogsQuery, IReadOnlyList<EmpAuditLogDto>>(
            new ListEmployeeAuditLogsQuery(User.GetIdpSubject(), employeeId),
            cancellationToken);
        return Ok(items);
    }

    /// <summary>Audit theo action (TIM publish/commit/close/unlock — RelatedId, EmployeeId có thể null).</summary>
    [HttpGet("audit-logs")]
    [Authorize]
    public async Task<IActionResult> ListAuditLogsByAction(
        [FromQuery] string action,
        [FromQuery] int take = 50,
        CancellationToken cancellationToken = default)
    {
        var items = await queries.DispatchAsync<ListAuditLogsByActionQuery, IReadOnlyList<EmpAuditLogDto>>(
            new ListAuditLogsByActionQuery(User.GetIdpSubject(), action, take),
            cancellationToken);
        return Ok(items);
    }
}
