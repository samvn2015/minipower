using Microsoft.Extensions.Hosting;

namespace Hrm.Domain.DependencyInjection;

public static class DomainLayerExtension
{
    public static IHostApplicationBuilder AddDomainLayer(this IHostApplicationBuilder builder)
    {
        // Đăng ký domain service stateless (nếu có)
        return builder;
    }
}
