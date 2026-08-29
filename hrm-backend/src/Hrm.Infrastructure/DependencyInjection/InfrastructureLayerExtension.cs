using Hrm.Domain.DependencyInjection;
using Hrm.Domain.Employees.Repositories;
using Hrm.Domain.Identity.Repositories;
using Hrm.Domain.Repositories;
using Hrm.Infrastructure.Persistence;
using Hrm.Infrastructure.Persistence.Repositories;
using Jarvis.BlobStoring.Extensions;
using Jarvis.Caching.Extensions;
using Jarvis.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Hrm.Infrastructure.DependencyInjection;

public static class InfrastructureLayerExtension
{
    public static IHostApplicationBuilder AddInfrastructureLayer(this IHostApplicationBuilder builder)
    {
        builder.AddDomainLayer();
        builder.AddJarvisCaching();
        builder.AddCoreBlobStoring();
        builder.AddEntityFramework();

        builder.Services.AddScoped<IAppUnitOfWork, AppUnitOfWork>();
        builder.Services.AddScoped<IIdentityAccountReadRepository, IdentityAccountReadRepository>();
        builder.Services.AddScoped<IIdentityAccountWriteRepository, IdentityAccountWriteRepository>();
        builder.Services.AddScoped<IIdentityAccountAdminRepository, IdentityAccountAdminRepository>();
        builder.Services.AddScoped<IEmployeeReadRepository, EmployeeReadRepository>();
        builder.Services.AddScoped<IEmployeeWriteRepository, EmployeeWriteRepository>();
        builder.Services.AddScoped<IOrgUnitReadRepository, OrgUnitReadRepository>();
        builder.Services.AddScoped<ILineManagerChangeRepository, LineManagerChangeRepository>();

        // Credentials: User Secrets / env (ConnectionStrings__AppDbContext). Không hard-code password.
        // Placeholder chỉ để đăng ký DI khi chưa có secret — ping/swagger vẫn chạy; mở DB thật cần OQ-DLV-003.
        var connectionString = builder.Configuration.GetConnectionString("AppDbContext");
        if (string.IsNullOrWhiteSpace(connectionString))
            connectionString = "Host=127.0.0.1;Port=5432;Database=hrm;Username=hrm_app";

        builder.Services.AddCoreDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString));

        return builder;
    }
}
