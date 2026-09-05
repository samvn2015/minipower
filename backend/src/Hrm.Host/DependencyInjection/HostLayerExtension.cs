using Asp.Versioning;
using Hrm.Application.Common;
using Hrm.Application.DependencyInjection;
using Hrm.Domain.Repositories;
using Hrm.Host.Services;
using Hrm.Infrastructure.DependencyInjection;
using Jarvis.Authentication;
using Jarvis.Authentication.Jwt;
using Jarvis.Domain;
using Jarvis.EntityFramework;
using Jarvis.HealthChecks;
using Jarvis.Mvc;
using Jarvis.Mvc.ApplicationBuilders;
using Jarvis.Mvc.ExceptionHandling;
using Jarvis.OpenTelemetry.Abstractions;
using Jarvis.OpenTelemetry.Extensions;
using Jarvis.Swashbuckle;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Hrm.Host.DependencyInjection;

/// <summary>
/// Composition root Host — Jarvis Mvc + Caching/EF (Infrastructure) + JWT + Swagger + Health + OTEL.
/// OIDC Authority thật = OQ-DLV-001. PostgreSQL connection = OQ-DLV-003.
/// Clone Jarvis: không có Multitenancy riêng / OpenTelemetry.DDD / AddCurrentUser.
/// </summary>
public static class HostLayerExtension
{
    public static IHostApplicationBuilder AddHostLayer(this IHostApplicationBuilder builder)
    {
        builder.Services
            .AddJarvisOpenTelemetry(builder.Configuration, services =>
            {
                services.AddScoped<IEnrichLogService, EnrichLogService>();
                services.AddScoped<IEnrichTraceService, EnrichTraceService>();
            })
            .ConfigureResource()
            .ConfigureLogging()
            .ConfigureTrace()
            .ConfigureMetric();

        builder.AddApplicationLayer();
        builder.AddInfrastructureLayer();

        builder.Services.AddSingleton<IHostRoleGate, HostRoleGate>();

        builder.AddCoreJson();
        builder.AddCoreCors();
        builder.AddCoreDomain();
        builder.AddCoreWebApi();

        builder.Services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ReportApiVersions = true;
        }).AddApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV";
            options.SubstituteApiVersionInUrl = true;
        });

        builder.Services.AddJarvisAuthentication(builder.Configuration, auth =>
        {
            auth.AddCoreJwtBearer(builder.Configuration, JwtBearerDefaults.AuthenticationScheme);
        });

        builder.AddCoreSwagger();
        builder.AddHealthChecks();

        return builder;
    }

    public static WebApplication UseHostLayer(this WebApplication app)
    {
        app.EnsureMigrateDb<IAppUnitOfWork>();
        app.UseCoreSwagger();
        if (!app.Environment.IsDevelopment())
            app.UseHttpsRedirection();
        app.UseCoreCors();
        app.UseJarvisOpenTelemetry();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseCoreMiddleware<ApiResponseWrapperMiddleware>();
        app.MapControllers();
        app.UseHealthChecks();
        return app;
    }
}
