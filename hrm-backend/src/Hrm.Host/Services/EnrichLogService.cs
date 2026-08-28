using Jarvis.OpenTelemetry.Abstractions;

namespace Hrm.Host.Services;

/// <summary>
/// Enrich log tùy chọn (clone Jarvis hiện tại không có OpenTelemetry.DDD / AddCurrentUser).
/// </summary>
public sealed class EnrichLogService : IEnrichLogService
{
    public Task<Dictionary<string, string>> ExtractAsync()
        => Task.FromResult(new Dictionary<string, string>());
}
