using Hrm.Domain.Payroll.Entities;
using Microsoft.EntityFrameworkCore;

namespace Hrm.Infrastructure.Persistence;

/// <summary>
/// ADR-011 W1c — DbContext riêng cho bounded context **PAY**.
///
/// Nối bằng role <c>hrm_app_pay</c> (<c>ConnectionStrings:PayDbContext</c>), chỉ thấy schema
/// <c>pay</c>. Đây là chỗ hàng rào W1b bắt đầu có tác dụng: mọi truy vấn sang schema module
/// khác bị PostgreSQL từ chối, không phụ thuộc code có đúng hay không (NFR-002).
///
/// **Không sở hữu migration.** <see cref="AppDbContext"/> vẫn là migration owner duy nhất,
/// chạy bằng role <c>hrm_migrator</c> — quyết định ③ii ở readiness gate, khớp DOC-17 §2.1.
/// Sinh migration phải chỉ rõ: <c>dotnet ef migrations add X --context AppDbContext</c>.
///
/// PAY chạm audit ở đúng một chỗ (<c>PayPayslipQueries</c>) và đó là **query** — không lệnh
/// nào vừa ghi entity PAY vừa ghi audit, nên audit ở lại <see cref="AppDbContext"/> mà không
/// tạo giao dịch bắc cầu. Nếu sau này có lệnh PAY cần ghi audit cùng transaction thì phải
/// map <c>EmpAuditLog</c> (schema <c>shared</c>) vào context này — role đã có sẵn quyền INSERT.
/// </summary>
public class PayDbContext(DbContextOptions<PayDbContext> options) : DbContext(options)
{
    public DbSet<PayPeriod> PayPeriods => Set<PayPeriod>();

    public DbSet<PayLine> PayLines => Set<PayLine>();

    public DbSet<PayRegulation> PayRegulations => Set<PayRegulation>();

    public DbSet<PayWorkdayCalendar> PayWorkdayCalendars => Set<PayWorkdayCalendar>();

    public DbSet<PayAllowanceCatalog> PayAllowanceCatalogs => Set<PayAllowanceCatalog>();

    public DbSet<PayContractAllowance> PayContractAllowances => Set<PayContractAllowance>();

    public DbSet<PayMonthlyAllowance> PayMonthlyAllowances => Set<PayMonthlyAllowance>();

    public DbSet<PayContractSalary> PayContractSalaries => Set<PayContractSalary>();

    public DbSet<PayExportOutbox> PayExportOutboxes => Set<PayExportOutbox>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Chỉ nạp cấu hình của PAY — KHÔNG dùng ApplyConfigurationsFromAssembly,
        // nếu không sẽ kéo cả 41 entity của 7 context vào đây.
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(PayDbContext).Assembly,
            type => type.Namespace == typeof(AppDbContext).Namespace + ".Configurations"
                    && type.Name.StartsWith("Pay", StringComparison.Ordinal));
    }
}
