using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hrm.Iam.Host.Controllers;

/// <summary>
/// IAM-AC-001 / IAM-TC-001 · DOC-12 <c>GET /iam/me</c>.
/// Không POST password login (ADR-007).
/// Authority OIDC thật = OQ-DLV-001 (IT). Local: ValidateIssuerSigningKey=false để Host start.
/// </summary>
[ApiController]
[Route("v1/iam")]
public sealed class MeController : ControllerBase
{
    [HttpGet("me")]
    [Authorize]
    public IActionResult Me()
    {
        var sub = User.FindFirstValue("sub")
                  ?? User.FindFirstValue(ClaimTypes.NameIdentifier)
                  ?? User.Identity?.Name;

        var roles = User.FindAll(ClaimTypes.Role)
            .Concat(User.FindAll("role"))
            .Select(c => c.Value)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        return Ok(new
        {
            sub,
            name = User.Identity?.Name,
            roles,
            note = "IAM DB SoT roles (ADR-002) — map đầy đủ khi có persistence PostgreSQL"
        });
    }
}
