using System.Security.Claims;
using Hrm.Application.Identity.Dtos;
using Hrm.Application.Identity.Queries;
using Hrm.Host.Extensions;
using Jarvis.Application.Contracts.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hrm.Host.Controllers;

/// <summary>
/// IAM-AC-001 / IAM-TC-001 · DOC-12 <c>GET /iam/me</c>.
/// Không POST password login (ADR-007).
/// Authority OIDC thật = OQ-DLV-001 (IT). Local: ValidateIssuerSigningKey=false để Host start.
/// </summary>
[ApiController]
[Route("v1/iam")]
public sealed class MeController(IAsyncQueryDispatcher queries) : ControllerBase
{
    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> Me(CancellationToken cancellationToken)
    {
        var subject = User.GetIdpSubject();

        var roleClaims = User.FindAll(ClaimTypes.Role)
            .Concat(User.FindAll("role"))
            .Select(static c => c.Value)
            .ToArray();

        var dto = await queries.DispatchAsync<GetCurrentUserQuery, CurrentUserDto>(
            new GetCurrentUserQuery(subject, User.Identity?.Name, User.GetEmailCty(), roleClaims),
            cancellationToken);

        return Ok(dto);
    }
}
