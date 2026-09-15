using Hrm.Domain.DependencyInjection;
using Hrm.Domain.Employees.Repositories;
using Hrm.Domain.Identity.Repositories;
using Hrm.Domain.Leave.Repositories;
using Hrm.Domain.Lifecycle.Repositories;
using Hrm.Domain.Payroll.Repositories;
using Hrm.Domain.Probation.Repositories;
using Hrm.Domain.Timekeeping.Repositories;
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
        builder.Services.AddScoped<IEducationLevelReadRepository, EducationLevelReadRepository>();
        builder.Services.AddScoped<ISeniorityRuleReadRepository, SeniorityRuleReadRepository>();
        builder.Services.AddScoped<IEmpAuditLogRepository, EmpAuditLogRepository>();
        builder.Services.AddScoped<ITimAuditLogRepository, TimAuditLogRepository>();
        builder.Services.AddScoped<IPayAuditLogRepository, PayAuditLogRepository>();
        builder.Services.AddScoped<IPrbAuditLogRepository, PrbAuditLogRepository>();
        builder.Services.AddScoped<ILifAuditLogRepository, LifAuditLogRepository>();
        builder.Services.AddScoped<ILineManagerChangeRepository, LineManagerChangeRepository>();
        builder.Services.AddScoped<ILeaveTypeReadRepository, LeaveTypeReadRepository>();
        builder.Services.AddScoped<ILeaveBalanceRepository, LeaveBalanceRepository>();
        builder.Services.AddScoped<ILeaveRequestRepository, LeaveRequestRepository>();
        builder.Services.AddScoped<ILeaveNotificationOutbox, LeaveNotificationOutbox>();
        builder.Services.AddScoped<ITimesheetTemplateRepository, TimesheetTemplateRepository>();
        builder.Services.AddScoped<ITimesheetImportRepository, TimesheetImportRepository>();
        builder.Services.AddScoped<IPayPeriodRepository, PayPeriodRepository>();
        builder.Services.AddScoped<IPayPeriodGate, PayPeriodRepository>();
        builder.Services.AddScoped<IPayRegulationReadRepository, PayRegulationReadRepository>();
        builder.Services.AddScoped<IPayWorkdayCalendarRepository, PayWorkdayCalendarRepository>();
        builder.Services.AddScoped<IPayAllowanceRepository, PayAllowanceRepository>();
        builder.Services.AddScoped<IPayContractSalaryRepository, PayContractSalaryRepository>();
        builder.Services.AddScoped<IPayExportOutboxRepository, PayExportOutboxRepository>();
        builder.Services.AddScoped<IProbationReminderRepository, ProbationReminderRepository>();
        builder.Services.AddScoped<IProbationMasterReadRepository, ProbationMasterReadRepository>();
        builder.Services.AddScoped<IProbationEvaluationRepository, ProbationEvaluationRepository>();
        builder.Services.AddScoped<ILifOffboardingRepository, LifOffboardingRepository>();
        builder.Services.AddScoped<ILifOffChecklistRepository, LifOffChecklistRepository>();
        builder.Services.AddScoped<ILifOnboardingRepository, LifOnboardingRepository>();
        builder.Services.AddScoped<ILifOnChecklistRepository, LifOnChecklistRepository>();

        // Credentials: User Secrets / env (ConnectionStrings__AppDbContext). Không hard-code password.
        // Placeholder chỉ để đăng ký DI khi chưa có secret — ping/swagger vẫn chạy; mở DB thật cần OQ-DLV-003.
        var connectionString = builder.Configuration.GetConnectionString("AppDbContext");
        if (string.IsNullOrWhiteSpace(connectionString))
            connectionString = "Host=127.0.0.1;Port=5432;Database=hrm;Username=hrm_app";

        builder.Services.AddCoreDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString));

        // ADR-011 W1c — PAY nối bằng role riêng (hrm_app_pay), chỉ thấy schema `pay`.
        // Thiếu cấu hình thì rơi về connection chung: hàng rào không có tác dụng nhưng
        // ứng dụng vẫn chạy — chủ ý, để W1c triển khai dần từng context.
        var payConnectionString = builder.Configuration.GetConnectionString("PayDbContext");
        if (string.IsNullOrWhiteSpace(payConnectionString))
            payConnectionString = connectionString;

        builder.Services.AddDbContext<PayDbContext>(options =>
            options.UseNpgsql(payConnectionString));

        AddContextConnection<IamDbContext>(builder, "IamDbContext", connectionString);
        AddContextConnection<LevDbContext>(builder, "LevDbContext", connectionString);
        AddContextConnection<EmpDbContext>(builder, "EmpDbContext", connectionString);
        AddContextConnection<TimDbContext>(builder, "TimDbContext", connectionString);
        AddContextConnection<PrbDbContext>(builder, "PrbDbContext", connectionString);
        AddContextConnection<LifDbContext>(builder, "LifDbContext", connectionString);

        return builder;
    }

    /// <summary>
    /// ADR-011 W1c — nối một bounded context bằng role riêng của nó.
    /// Thiếu cấu hình thì rơi về connection chung: app vẫn chạy, hàng rào chưa hoạt động.
    /// </summary>
    private static void AddContextConnection<TContext>(
        IHostApplicationBuilder builder,
        string name,
        string fallback)
        where TContext : DbContext
    {
        var cs = builder.Configuration.GetConnectionString(name);
        if (string.IsNullOrWhiteSpace(cs))
            cs = fallback;

        builder.Services.AddDbContext<TContext>(options => options.UseNpgsql(cs));
    }
}
