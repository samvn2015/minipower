using Asp.Versioning;
using Hrm.Iam.Host.DependencyInjection;
using Jarvis.Authentication;
using Jarvis.Authentication.Jwt;
using Jarvis.Domain;
using Jarvis.Mvc;
using Jarvis.Mvc.ApplicationBuilders;
using Jarvis.Mvc.ExceptionHandling;
using Jarvis.Swashbuckle;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Hrm.Iam.Host.DependencyInjection;

/// <summary>
/// Host IAM tối thiểu: Jarvis Mvc + JWT Bearer + Swagger.
/// Chưa EF/PostgreSQL (OQ-DLV-003 connection). Chưa OIDC Authority thật (OQ-DLV-001).
/// </summary>
public static class HostLayerExtension
{
    public static IHostApplicationBuilder AddHostLayer(this IHostApplicationBuilder builder)
    {
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
        return builder;
    }

    public static WebApplication UseHostLayer(this WebApplication app)
    {
        app.UseCoreSwagger();
        app.UseHttpsRedirection();
        app.UseCoreCors();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseCoreMiddleware<ApiResponseWrapperMiddleware>();
        app.MapControllers();
        return app;
    }
}
