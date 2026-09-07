using Hrm.Domain.Lifecycle.Entities;
using Hrm.Domain.Employees.Entities;
using Microsoft.EntityFrameworkCore;

namespace Hrm.Infrastructure.Persistence;

/// <summary>
/// ADR-011 W1c — DbContext riêng cho bounded context <b>LIF</b>
/// (role <c>hrm_app_lif</c>, <c>ConnectionStrings:LifDbContext</c>, schema <c>lif</c>).
/// Không sở hữu migration — xem <see cref="PayDbContext"/>.
///
/// Map thêm <c>EmpAuditLog</c> (schema <c>shared</c>) theo quyết định ①a + A: audit ghi bằng
/// context của chính module, nên lệnh nghiệp vụ và dòng audit nằm trong <b>một</b> transaction.
/// Role đã có sẵn INSERT trên <c>shared</c>.
/// </summary>
public class LifDbContext(DbContextOptions<LifDbContext> options) : DbContext(options)
{
    public DbSet<LifOffboardingCase> LifOffboardingCases => Set<LifOffboardingCase>();

    public DbSet<LifOffChecklistItem> LifOffChecklistItems => Set<LifOffChecklistItem>();

    public DbSet<LifOffChecklistTick> LifOffChecklistTicks => Set<LifOffChecklistTick>();

    public DbSet<LifAccessLockOutbox> LifAccessLockOutboxes => Set<LifAccessLockOutbox>();

    public DbSet<LifOnboardingCase> LifOnboardingCases => Set<LifOnboardingCase>();

    public DbSet<LifOnChecklistItem> LifOnChecklistItems => Set<LifOnChecklistItem>();

    public DbSet<LifOnChecklistTick> LifOnChecklistTicks => Set<LifOnChecklistTick>();

    /// <summary>Bảng audit dùng chung ở schema <c>shared</c> (①a).</summary>
    public DbSet<EmpAuditLog> EmpAuditLogs => Set<EmpAuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(LifDbContext).Assembly,
            type => type.Namespace == typeof(AppDbContext).Namespace + ".Configurations"
                    && (type.Name.StartsWith("Lif", StringComparison.Ordinal) 
                        || type.Name.StartsWith("EmpAuditLog", StringComparison.Ordinal)));
    }
}
