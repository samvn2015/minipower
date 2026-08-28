using Jarvis.OpenTelemetry.Abstractions;

namespace Hrm.Host.Services;

/// <summary>
/// Enrich trace tùy chọn (clone Jarvis hiện tại không có OpenTelemetry.DDD / AddCurrentUser).
/// </summary>
public sealed class EnrichTraceService : IEnrichTraceService
{
    public Task<Dictionary<string, string>> ExtractAsync()
        => Task.FromResult(new Dictionary<string, string>());
}
