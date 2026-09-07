using Hrm.Domain.Timekeeping.Entities;
using Hrm.Domain.Employees.Entities;
using Microsoft.EntityFrameworkCore;

namespace Hrm.Infrastructure.Persistence;

/// <summary>
/// ADR-011 W1c — DbContext riêng cho bounded context <b>TIM</b>
/// (role <c>hrm_app_tim</c>, <c>ConnectionStrings:TimDbContext</c>, schema <c>tim</c>).
/// Không sở hữu migration — xem <see cref="PayDbContext"/>.
///
/// Map thêm <c>EmpAuditLog</c> (schema <c>shared</c>) theo quyết định ①a + A: audit ghi bằng
/// context của chính module, nên lệnh nghiệp vụ và dòng audit nằm trong <b>một</b> transaction.
/// Role đã có sẵn INSERT trên <c>shared</c>.
/// </summary>
public class TimDbContext(DbContextOptions<TimDbContext> options) : DbContext(options)
{
    public DbSet<TimesheetTemplateVersion> TimesheetTemplateVersions => Set<TimesheetTemplateVersion>();

    public DbSet<TimesheetTemplateColumn> TimesheetTemplateColumns => Set<TimesheetTemplateColumn>();

    public DbSet<TimesheetImportBatch> TimesheetImportBatches => Set<TimesheetImportBatch>();

    public DbSet<TimesheetImportRow> TimesheetImportRows => Set<TimesheetImportRow>();

    public DbSet<TimesheetPeriod> TimesheetPeriods => Set<TimesheetPeriod>();

    public DbSet<TimesheetLine> TimesheetLines => Set<TimesheetLine>();

    /// <summary>Bảng audit dùng chung ở schema <c>shared</c> (①a).</summary>
    public DbSet<EmpAuditLog> EmpAuditLogs => Set<EmpAuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(TimDbContext).Assembly,
            type => type.Namespace == typeof(AppDbContext).Namespace + ".Configurations"
                    && (type.Name.StartsWith("Timesheet", StringComparison.Ordinal) 
                        || type.Name.StartsWith("EmpAuditLog", StringComparison.Ordinal)));
    }
}
