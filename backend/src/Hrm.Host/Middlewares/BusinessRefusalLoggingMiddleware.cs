using Hrm.Host.Extensions;
using Jarvis.Domain.Shared.ExceptionHandling;

namespace Hrm.Host.Middlewares;

/// <summary>
/// Code-review S4 — ghi log **lý do** khi hệ thống từ chối một thao tác nghiệp vụ.
///
/// Trước đây client nhận đủ lý do trong <c>error.systemMessage</c>, nhưng log chỉ có
/// <c>Request finished … 400</c>. Sau sự cố, support không lần được vì sao một thao tác
/// bị chặn — trong khi đó chính là câu hỏi hay gặp nhất (*"sao tôi không chạy được lương?"*).
///
/// Đặt ở **một chỗ** thay vì rải log vào từng handler: mọi từ chối đều đi qua
/// <see cref="BusinessException"/> của Jarvis, nên bắt ở đây là đủ và không sinh nhiễu.
/// Middleware **không nuốt lỗi** — ghi xong ném lại cho
/// <c>ApiResponseWrapperMiddleware</c> dựng response như cũ.
///
/// Mức log: <see cref="LogLevel.Warning"/> cho từ chối nghiệp vụ (hệ thống vẫn đúng),
/// không phải Error — Error dành cho lỗi ngoài dự kiến.
/// </summary>
public sealed class BusinessRefusalLoggingMiddleware(
    RequestDelegate next,
    ILogger<BusinessRefusalLoggingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context).ConfigureAwait(false);
        }
        catch (BusinessException ex)
        {
            // Structured template — trường tách riêng để lọc được trên hệ thống log,
            // không nội suy vào chuỗi.
            logger.LogWarning(
                "Từ chối nghiệp vụ {Method} {Path} · code={Code} · actor={Actor} · lý do: {Reason}",
                context.Request.Method,
                context.Request.Path.Value,
                ex.Code,
                context.User.GetIdpSubject() ?? "(chưa xác thực)",
                ex.SystemMessage ?? ex.Message);

            throw;
        }
    }
}
