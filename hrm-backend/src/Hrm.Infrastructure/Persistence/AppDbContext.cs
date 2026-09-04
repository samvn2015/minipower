using Hrm.Domain.Employees.Entities;
using Hrm.Domain.Identity.Entities;
using Hrm.Domain.Leave.Entities;
using Hrm.Domain.Lifecycle.Entities;
using Hrm.Domain.Payroll.Entities;
using Hrm.Domain.Probation.Entities;
using Hrm.Domain.Timekeeping.Entities;
using Jarvis.EntityFramework.DataStorages;
using Microsoft.EntityFrameworkCore;

namespace Hrm.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options)
    : BaseStorageContext<AppDbContext>(options)
{
    public DbSet<IdentityAccount> IdentityAccounts => Set<IdentityAccount>();

    public DbSet<Role> Roles => Set<Role>();

    public DbSet<AccountRole> AccountRoles => Set<AccountRole>();

    public DbSet<Employee> Employees => Set<Employee>();

    public DbSet<OrgUnit> OrgUnits => Set<OrgUnit>();

    public DbSet<EducationLevel> EducationLevels => Set<EducationLevel>();

    public DbSet<SeniorityRule> SeniorityRules => Set<SeniorityRule>();

    public DbSet<EmpAuditLog> EmpAuditLogs => Set<EmpAuditLog>();

    public DbSet<EmployeeContract> EmployeeContracts => Set<EmployeeContract>();

    public DbSet<LineManagerChangeRequest> LineManagerChangeRequests => Set<LineManagerChangeRequest>();

    public DbSet<LeaveType> LeaveTypes => Set<LeaveType>();

    public DbSet<LeaveBalance> LeaveBalances => Set<LeaveBalance>();

    public DbSet<LeaveRequest> LeaveRequests => Set<LeaveRequest>();

    public DbSet<TimesheetTemplateVersion> TimesheetTemplateVersions => Set<TimesheetTemplateVersion>();

    public DbSet<TimesheetTemplateColumn> TimesheetTemplateColumns => Set<TimesheetTemplateColumn>();

    public DbSet<TimesheetImportBatch> TimesheetImportBatches => Set<TimesheetImportBatch>();

    public DbSet<TimesheetImportRow> TimesheetImportRows => Set<TimesheetImportRow>();

    public DbSet<TimesheetPeriod> TimesheetPeriods => Set<TimesheetPeriod>();

    public DbSet<TimesheetLine> TimesheetLines => Set<TimesheetLine>();

    public DbSet<PayPeriod> PayPeriods => Set<PayPeriod>();

    public DbSet<PayLine> PayLines => Set<PayLine>();

    public DbSet<PayRegulation> PayRegulations => Set<PayRegulation>();

    public DbSet<PayWorkdayCalendar> PayWorkdayCalendars => Set<PayWorkdayCalendar>();

    public DbSet<PayAllowanceCatalog> PayAllowanceCatalogs => Set<PayAllowanceCatalog>();

    public DbSet<PayContractAllowance> PayContractAllowances => Set<PayContractAllowance>();

    public DbSet<PayMonthlyAllowance> PayMonthlyAllowances => Set<PayMonthlyAllowance>();

    public DbSet<PayContractSalary> PayContractSalaries => Set<PayContractSalary>();

    public DbSet<PayExportOutbox> PayExportOutboxes => Set<PayExportOutbox>();

    public DbSet<ProbationReminder> ProbationReminders => Set<ProbationReminder>();

    public DbSet<ProbationOutcome> ProbationOutcomes => Set<ProbationOutcome>();

    public DbSet<ProbationCriterion> ProbationCriteria => Set<ProbationCriterion>();

    public DbSet<ProbationExtendDuration> ProbationExtendDurations => Set<ProbationExtendDuration>();

    public DbSet<ProbationEvaluation> ProbationEvaluations => Set<ProbationEvaluation>();

    public DbSet<LifOffboardingCase> LifOffboardingCases => Set<LifOffboardingCase>();

    public DbSet<LifOffChecklistItem> LifOffChecklistItems => Set<LifOffChecklistItem>();

    public DbSet<LifOffChecklistTick> LifOffChecklistTicks => Set<LifOffChecklistTick>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
