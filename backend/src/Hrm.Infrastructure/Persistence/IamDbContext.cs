using Hrm.Domain.Identity.Entities;
using Hrm.Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Hrm.Infrastructure.Persistence;

/// <summary>
/// ADR-011 W1c — DbContext riêng cho bounded context **IAM** (role <c>hrm_app_iam</c>,
/// <c>ConnectionStrings:IamDbContext</c>). Không sở hữu migration — xem <see cref="PayDbContext"/>.
///
/// IAM **không** ghi <c>EmpAuditLog</c> (kiểm 2026-09-07: chỉ EMP, TIM, PAY, PRB, LIF ghi),
/// nên tách context không tạo giao dịch bắc cầu.
/// </summary>
public class IamDbContext(DbContextOptions<IamDbContext> options) : DbContext(options)
{
    public DbSet<IdentityAccount> IdentityAccounts => Set<IdentityAccount>();

    public DbSet<Role> Roles => Set<Role>();

    public DbSet<AccountRole> AccountRoles => Set<AccountRole>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Ba cấu hình IAM không chung tiền tố tên → liệt kê tường minh,
        // KHÔNG ApplyConfigurationsFromAssembly (sẽ kéo cả 41 entity vào).
        modelBuilder.ApplyConfiguration(new IdentityAccountConfiguration());
        modelBuilder.ApplyConfiguration(new RoleConfiguration());
        modelBuilder.ApplyConfiguration(new AccountRoleConfiguration());
    }
}
