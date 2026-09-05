using Hrm.Application.Employees.Commands;
using Hrm.Application.Employees.Dtos;
using Hrm.Application.Employees.Queries;
using Hrm.Domain.Employees.Repositories;
using Hrm.Domain.Shared.Constants;
using Hrm.Host.Extensions;
using Jarvis.Application.Contracts.Commands;
using Jarvis.Application.Contracts.Queries;
using Jarvis.Domain.Shared.ExceptionHandling;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hrm.Host.Controllers;

/// <summary>
/// EMP — DOC-12 · SCR-001 list · SCR-002 create/patch · SCR-005 submit LM.
/// </summary>
[ApiController]
[Route("v1/emp/employees")]
[Authorize]
public sealed class EmployeesController(
    IAsyncQueryDispatcher queries,
    IAsyncCommandDispatcher commands) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List(CancellationToken cancellationToken)
    {
        var items = await queries.DispatchAsync<ListEmployeesQuery, IReadOnlyList<EmployeeListItemDto>>(
            new ListEmployeesQuery(User.GetIdpSubject()),
            cancellationToken);
        return Ok(items);
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetMe(CancellationToken cancellationToken)
    {
        var dto = await queries.DispatchAsync<GetMyEmployeeQuery, EmployeeDto>(
            new GetMyEmployeeQuery(User.GetIdpSubject()),
            cancellationToken);
        return Ok(dto);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateEmployeeRequest body,
        CancellationToken cancellationToken)
    {
        var result = await commands.DispatchAsync<CreateEmployeeCommand, EmployeeCreateResult>(
            new CreateEmployeeCommand(
                User.GetIdpSubject(),
                body.EmployeeCode,
                body.FullName,
                body.Cccd,
                body.EmailCty,
                body.TaxId,
                body.OrgUnitCode,
                body.EducationLevelCode,
                body.SeniorityStartDate,
                MapContract(body.Contract)),
            cancellationToken);
        return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
    }

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
        if (body.LineManagerEmployeeId.HasValue)
        {
            throw new BadRequestException(
                HrmErrorCodes.BadRequest,
                "Cấm đổi LM trên SCR-002 — dùng SCR-005/006 (EMP-FR-006).");
        }

        var result = await commands.DispatchAsync<UpdateEmployeeCommand, EmployeeUpdateResult>(
            new UpdateEmployeeCommand(
                id,
                User.GetIdpSubject(),
                body.FullName,
                body.EmailCty,
                body.Cccd,
                body.TaxId,
                body.OrgUnitCode,
                body.EducationLevelCode,
                body.SeniorityStartDate,
                MapContract(body.Contract)),
            cancellationToken);
        return Ok(result);
    }

    private static EmployeeContractUpsert? MapContract(EmployeeContractRequest? contract) =>
        contract is null
            ? null
            : new EmployeeContractUpsert(
                contract.ContractType,
                contract.StartDate,
                contract.EndDate,
                contract.IsProbation);

    public sealed record CreateEmployeeRequest(
        string EmployeeCode,
        string? FullName,
        string? Cccd,
        string? EmailCty,
        string? TaxId,
        string OrgUnitCode,
        string? EducationLevelCode,
        DateOnly? SeniorityStartDate,
        EmployeeContractRequest? Contract);

    public sealed record UpdateEmployeeRequest(
        string? FullName,
        string? EmailCty,
        string? Cccd,
        string? TaxId,
        string? OrgUnitCode,
        string? EducationLevelCode,
        DateOnly? SeniorityStartDate,
        EmployeeContractRequest? Contract,
        Guid? LineManagerEmployeeId);

    public sealed record EmployeeContractRequest(
        string ContractType,
        DateOnly StartDate,
        DateOnly? EndDate,
        bool IsProbation);
}
