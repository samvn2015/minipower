using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Hrm.Host.Controllers;

/// <summary>
/// Dev-only JWT để E2E local (OQ-DLV-001 chưa có Lark). Không bật Production.
/// </summary>
[ApiController]
[Route("dev")]
public sealed class DevAuthController(IConfiguration configuration, IHostEnvironment environment) : ControllerBase
{
    public const string DefaultSigningKey = "hrm-local-dev-secret-min-32-chars!!";

    [HttpGet("token")]
    [ProducesResponseType(typeof(DevTokenResponse), StatusCodes.Status200OK)]
    public IActionResult GetToken([FromQuery] string sub = "local-dev")
    {
        if (!environment.IsDevelopment())
            return NotFound();

        if (string.IsNullOrWhiteSpace(sub))
            return BadRequest("sub bắt buộc.");

        var signingKey = ResolveSigningKey();
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.AddHours(8);

        var token = new JwtSecurityToken(
            claims: [new Claim("sub", sub.Trim())],
            expires: expires,
            signingCredentials: credentials);

        return Ok(new DevTokenResponse(
            new JwtSecurityTokenHandler().WriteToken(token),
            sub.Trim(),
            expires));
    }

    private string ResolveSigningKey()
    {
        var keys = configuration.GetSection("Authentication:Jwt:Bearer:IssuerSigningKeys").Get<string[]>();
        var key = keys?.FirstOrDefault(static k => !string.IsNullOrWhiteSpace(k));
        return string.IsNullOrWhiteSpace(key) ? DefaultSigningKey : key;
    }

    public sealed record DevTokenResponse(string AccessToken, string Sub, DateTime ExpiresUtc);
}
