using Hrm.Application.Employees.Commands;
using Hrm.Application.Employees.Dtos;
using Hrm.Application.Employees.Queries;
using Hrm.Host.Extensions;
using Jarvis.Application.Contracts.Commands;
using Jarvis.Application.Contracts.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hrm.Host.Controllers;

/// <summary>
/// EMP skeleton — DOC-12 <c>GET/PATCH /emp/employees/{id}</c>.
/// </summary>
[ApiController]
[Route("v1/emp/employees")]
[Authorize]
public sealed class EmployeesController(
    IAsyncQueryDispatcher queries,
    IAsyncCommandDispatcher commands) : ControllerBase
{
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id, CancellationToken cancellationToken)
    {
        var dto = await queries.DispatchAsync<GetEmployeeQuery, EmployeeDto>(
            new GetEmployeeQuery(id, User.GetIdpSubject()),
            cancellationToken);
        return Ok(dto);
    }

    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> Patch(
        Guid id,
        [FromBody] UpdateEmployeeRequest body,
        CancellationToken cancellationToken)
    {
        var result = await commands.DispatchAsync<UpdateEmployeeCommand, EmployeeUpdateResult>(
            new UpdateEmployeeCommand(
                id,
                User.GetIdpSubject(),
                body.FullName,
                body.EmailCty,
                body.Cccd),
            cancellationToken);
        return Ok(result);
    }

    public sealed record UpdateEmployeeRequest(string? FullName, string? EmailCty, string? Cccd);
}
