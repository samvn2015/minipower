using Hrm.Domain.Employees.Entities;
using Hrm.Domain.Employees.Entities;
using Microsoft.EntityFrameworkCore;

namespace Hrm.Infrastructure.Persistence;

/// <summary>
/// ADR-011 W1c — DbContext riêng cho bounded context <b>EMP</b>
/// (role <c>hrm_app_emp</c>, <c>ConnectionStrings:EmpDbContext</c>, schema <c>emp</c>).
/// Không sở hữu migration — xem <see cref="PayDbContext"/>.
///
/// Map thêm <c>EmpAuditLog</c> (schema <c>shared</c>) theo quyết định ①a + A: audit ghi bằng
/// context của chính module, nên lệnh nghiệp vụ và dòng audit nằm trong <b>một</b> transaction.
/// Role đã có sẵn INSERT trên <c>shared</c>.
/// </summary>
public class EmpDbContext(DbContextOptions<EmpDbContext> options) : DbContext(options)
{
    public DbSet<Employee> Employees => Set<Employee>();

    public DbSet<OrgUnit> OrgUnits => Set<OrgUnit>();

    public DbSet<EducationLevel> EducationLevels => Set<EducationLevel>();

    public DbSet<SeniorityRule> SeniorityRules => Set<SeniorityRule>();

    public DbSet<EmployeeContract> EmployeeContracts => Set<EmployeeContract>();

    public DbSet<LineManagerChangeRequest> LineManagerChangeRequests => Set<LineManagerChangeRequest>();

    /// <summary>Bảng audit dùng chung ở schema <c>shared</c> (①a).</summary>
    public DbSet<EmpAuditLog> EmpAuditLogs => Set<EmpAuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(EmpDbContext).Assembly,
            type => type.Namespace == typeof(AppDbContext).Namespace + ".Configurations"
                    && (type.Name.StartsWith("Employee", StringComparison.Ordinal) 
                        || type.Name.StartsWith("EducationLevel", StringComparison.Ordinal) 
                        || type.Name.StartsWith("OrgUnit", StringComparison.Ordinal) 
                        || type.Name.StartsWith("SeniorityRule", StringComparison.Ordinal) 
                        || type.Name.StartsWith("LineManagerChange", StringComparison.Ordinal) 
                        || type.Name.StartsWith("EmpAuditLog", StringComparison.Ordinal)));
    }
}
