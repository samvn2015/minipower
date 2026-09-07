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
    public IActionResult GetToken([FromQuery] string sub = "local-dev", [FromQuery] string? email = null)
    {
        if (!environment.IsDevelopment())
            return NotFound();

        if (string.IsNullOrWhiteSpace(sub))
            return BadRequest("sub bắt buộc.");

        return Ok(IssueToken(sub, email));
    }

    /// <summary>
    /// Đăng nhập username/password cho DEV/UAT — đối chiếu danh sách <c>DevAuth:Accounts</c>
    /// trong cấu hình. KHÔNG đọc/ghi password trong DB (DOC-11 §3.1 giữ nguyên: không lưu
    /// password hash). Prod vẫn Lark SSO (ADR-007) vì endpoint 404 ngoài Development.
    /// </summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(DevTokenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult Login([FromBody] DevLoginRequest request)
    {
        if (!environment.IsDevelopment())
            return NotFound();

        if (string.IsNullOrWhiteSpace(request?.Username) || string.IsNullOrWhiteSpace(request.Password))
            return BadRequest("username và password bắt buộc.");

        var account = configuration
            .GetSection("DevAuth:Accounts")
            .Get<DevAccountOptions[]>()?
            .FirstOrDefault(a =>
                string.Equals(a.Username?.Trim(), request.Username.Trim(), StringComparison.OrdinalIgnoreCase)
                && string.Equals(a.Password, request.Password, StringComparison.Ordinal));

        if (account is null || string.IsNullOrWhiteSpace(account.Sub))
            return Unauthorized("Sai tài khoản hoặc mật khẩu.");

        return Ok(IssueToken(account.Sub, account.Email ?? account.Username));
    }

    private DevTokenResponse IssueToken(string sub, string? email)
    {
        var signingKey = ResolveSigningKey();
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.AddHours(8);

        var claims = new List<Claim> { new("sub", sub.Trim()) };
        if (!string.IsNullOrWhiteSpace(email))
            claims.Add(new Claim("email", email.Trim()));

        var token = new JwtSecurityToken(
            claims: claims,
            expires: expires,
            signingCredentials: credentials);

        return new DevTokenResponse(
            new JwtSecurityTokenHandler().WriteToken(token),
            sub.Trim(),
            expires);
    }

    private string ResolveSigningKey()
    {
        var keys = configuration.GetSection("Authentication:Jwt:Bearer:IssuerSigningKeys").Get<string[]>();
        var key = keys?.FirstOrDefault(static k => !string.IsNullOrWhiteSpace(k));
        return string.IsNullOrWhiteSpace(key) ? DefaultSigningKey : key;
    }

    public sealed record DevTokenResponse(string AccessToken, string Sub, DateTime ExpiresUtc);

    public sealed record DevLoginRequest(string Username, string Password);

    private sealed class DevAccountOptions
    {
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? Sub { get; set; }
        public string? Email { get; set; }
    }
}
