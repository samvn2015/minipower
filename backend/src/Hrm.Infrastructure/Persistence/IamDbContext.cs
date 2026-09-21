using Hrm.Domain.Identity.Entities;
using Hrm.Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Hrm.Infrastructure.Persistence;

/// <summary>
/// ADR-011 W1c — DbContext riêng cho bounded context **IAM** (role <c>hrm_app_iam</c>,
/// <c>ConnectionStrings:IamDbContext</c>). Không sở hữu migration — xem <see cref="PayDbContext"/>.
///
/// IAM ghi <c>EmpAuditLog</c> (schema <c>shared</c>) cho gán/thu role và disable — bổ sung
/// 2026-09-18 (doc-review pass 3 B1). Đây cũng là chỗ audit đăng nhập/khoá sẽ ghi khi CR-002
/// mở code (ADR-012, NFR-005). Handler bọc bằng <c>IIamAtomicScope</c> — một transaction (pass 4 M1).
/// </summary>
public class IamDbContext(DbContextOptions<IamDbContext> options) : DbContext(options)
{
    public DbSet<IdentityAccount> IdentityAccounts => Set<IdentityAccount>();

    public DbSet<Role> Roles => Set<Role>();

    public DbSet<AccountRole> AccountRoles => Set<AccountRole>();

    /// <summary>Bảng audit dùng chung ở schema <c>shared</c> (①a + phương án A).</summary>
    public DbSet<Domain.Employees.Entities.EmpAuditLog> EmpAuditLogs =>
        Set<Domain.Employees.Entities.EmpAuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Ba cấu hình IAM không chung tiền tố tên → liệt kê tường minh,
        // KHÔNG ApplyConfigurationsFromAssembly (sẽ kéo cả 41 entity vào).
        modelBuilder.ApplyConfiguration(new IdentityAccountConfiguration());
        modelBuilder.ApplyConfiguration(new RoleConfiguration());
        modelBuilder.ApplyConfiguration(new AccountRoleConfiguration());
        modelBuilder.ApplyConfiguration(new EmpAuditLogConfiguration());
    }
}
