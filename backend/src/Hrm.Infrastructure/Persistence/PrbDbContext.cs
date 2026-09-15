using Hrm.Domain.Probation.Entities;
using Hrm.Domain.Employees.Entities;
using Microsoft.EntityFrameworkCore;

namespace Hrm.Infrastructure.Persistence;

/// <summary>
/// ADR-011 W1c — DbContext riêng cho bounded context <b>PRB</b>
/// (role <c>hrm_app_prb</c>, <c>ConnectionStrings:PrbDbContext</c>, schema <c>prb</c>).
/// Không sở hữu migration — xem <see cref="PayDbContext"/>.
///
/// Map thêm <c>EmpAuditLog</c> (schema <c>shared</c>) theo quyết định ①a + A: audit ghi bằng
/// context của chính module, nên lệnh nghiệp vụ và dòng audit nằm trong <b>một</b> transaction.
/// Role đã có sẵn INSERT trên <c>shared</c>.
/// </summary>
public class PrbDbContext(DbContextOptions<PrbDbContext> options) : DbContext(options)
{
    public DbSet<ProbationReminder> ProbationReminders => Set<ProbationReminder>();

    public DbSet<ProbationOutcome> ProbationOutcomes => Set<ProbationOutcome>();

    public DbSet<ProbationCriterion> ProbationCriteria => Set<ProbationCriterion>();

    public DbSet<ProbationExtendDuration> ProbationExtendDurations => Set<ProbationExtendDuration>();

    public DbSet<ProbationEvaluation> ProbationEvaluations => Set<ProbationEvaluation>();

    /// <summary>Bảng audit dùng chung ở schema <c>shared</c> (①a).</summary>
    public DbSet<EmpAuditLog> EmpAuditLogs => Set<EmpAuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(PrbDbContext).Assembly,
            type => type.Namespace == typeof(AppDbContext).Namespace + ".Configurations"
                    && (type.Name.StartsWith("Probation", StringComparison.Ordinal) 
                        || type.Name.StartsWith("EmpAuditLog", StringComparison.Ordinal)));
    }
}
