using System.Security.Claims;

namespace Hrm.Host.Extensions;

internal static class ClaimsPrincipalExtensions
{
    public static string? GetIdpSubject(this ClaimsPrincipal user) =>
        user.FindFirstValue("sub")
        ?? user.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? user.Identity?.Name;

    public static string? GetEmailCty(this ClaimsPrincipal user) =>
        user.FindFirstValue("email")
        ?? user.FindFirstValue(ClaimTypes.Email);
}
