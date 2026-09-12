using System.Security.Claims;
using Hrm.Domain.Shared.Constants;
using Jarvis.Domain.Shared.ExceptionHandling;

namespace Hrm.Host.Extensions;

internal static class ClaimsPrincipalExtensions
{
    public static string? GetIdpSubject(this ClaimsPrincipal user) =>
        user.FindFirstValue("sub")
        ?? user.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? user.Identity?.Name;

    /// <summary>
    /// Code-review S2 — lấy <c>sub</c> và **bắt buộc có**.
    ///
    /// <c>[Authorize]</c> chỉ bảo đảm token hợp lệ, **không** bảo đảm có claim <c>sub</c>.
    /// Trước đây <see cref="GetIdpSubject"/> trả <c>null</c> được truyền thẳng vào tham số
    /// <c>string ActorIdpSubject</c> non-nullable (52 cảnh báo CS8604) — null chảy xuống
    /// tận câu truy vấn, khớp nhầm hoặc ném NullReference ở tầng sâu, khó lần.
    ///
    /// Chặn ngay tại biên: token thiếu định danh thì **401**, không phải lỗi 500 ở tầng dưới.
    /// </summary>
    /// <exception cref="UnauthorizedException">Token không mang định danh người dùng.</exception>
    public static string RequireIdpSubject(this ClaimsPrincipal user) =>
        user.GetIdpSubject()
        ?? throw new UnauthorizedException(
            HrmErrorCodes.Unauthorized,
            "Token không mang định danh người dùng (thiếu claim sub).");

    public static string? GetEmailCty(this ClaimsPrincipal user) =>
        user.FindFirstValue("email")
        ?? user.FindFirstValue(ClaimTypes.Email);
}
