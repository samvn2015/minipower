using Hrm.Application.Identity.Dtos;
using Hrm.Application.Identity.Queries;
using Jarvis.Application;
using Jarvis.Application.Contracts.Queries;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Hrm.Application.DependencyInjection;

public static class ApplicationLayerExtension
{
    public static IHostApplicationBuilder AddApplicationLayer(this IHostApplicationBuilder builder)
    {
        builder.AddCoreApplication();
        builder.Services.AddScoped<IAsyncQueryHandler<GetCurrentUserQuery, CurrentUserDto>, GetCurrentUserQueryHandler>();
        return builder;
    }
}
