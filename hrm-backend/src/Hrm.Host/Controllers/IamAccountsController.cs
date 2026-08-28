using Hrm.Application.Identity.Admin.Commands;
using Hrm.Application.Identity.Admin.Dtos;
using Hrm.Application.Identity.Admin.Queries;
using Hrm.Host.Extensions;
using Jarvis.Application.Contracts.Commands;
using Jarvis.Application.Contracts.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hrm.Host.Controllers;

/// <summary>
/// IAM-SCR-003 (gán/gỡ role) · IAM-SCR-004 (disable login). Không khóa Git/CRM (LIF).
/// </summary>
[ApiController]
[Route("v1/iam/accounts")]
[Authorize]
public sealed class IamAccountsController(
    IAsyncQueryDispatcher queries,
    IAsyncCommandDispatcher commands) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List(CancellationToken cancellationToken)
    {
        var items = await queries.DispatchAsync<ListIdentityAccountsQuery, IReadOnlyList<IdentityAccountDto>>(
            new ListIdentityAccountsQuery(User.GetIdpSubject()),
            cancellationToken);
        return Ok(items);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id, CancellationToken cancellationToken)
    {
        var dto = await queries.DispatchAsync<GetIdentityAccountQuery, IdentityAccountDto>(
            new GetIdentityAccountQuery(id, User.GetIdpSubject()),
            cancellationToken);
        return Ok(dto);
    }

    [HttpPost("{id:guid}/roles")]
    public async Task<IActionResult> AssignRole(
        Guid id,
        [FromBody] AssignRoleRequest body,
        CancellationToken cancellationToken)
    {
        var result = await commands.DispatchAsync<AssignAccountRoleCommand, IdentityAccountAdminResult>(
            new AssignAccountRoleCommand(id, body.RoleCode, User.GetIdpSubject()),
            cancellationToken);
        return Ok(result);
    }

    [HttpDelete("{id:guid}/roles/{roleCode}")]
    public async Task<IActionResult> RemoveRole(
        Guid id,
        string roleCode,
        CancellationToken cancellationToken)
    {
        var result = await commands.DispatchAsync<RemoveAccountRoleCommand, IdentityAccountAdminResult>(
            new RemoveAccountRoleCommand(id, roleCode, User.GetIdpSubject()),
            cancellationToken);
        return Ok(result);
    }

    [HttpPost("{id:guid}/disable")]
    public async Task<IActionResult> Disable(Guid id, CancellationToken cancellationToken)
    {
        var result = await commands.DispatchAsync<DisableIdentityAccountCommand, IdentityAccountAdminResult>(
            new DisableIdentityAccountCommand(id, User.GetIdpSubject()),
            cancellationToken);
        return Ok(result);
    }

    public sealed record AssignRoleRequest(string RoleCode);
}
