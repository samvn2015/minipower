using Hrm.Domain.Employees.Entities;
using Hrm.Domain.Identity.Entities;
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

    public DbSet<EmployeeContract> EmployeeContracts => Set<EmployeeContract>();

    public DbSet<LineManagerChangeRequest> LineManagerChangeRequests => Set<LineManagerChangeRequest>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
