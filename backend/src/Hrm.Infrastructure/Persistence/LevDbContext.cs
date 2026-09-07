using Hrm.Domain.Leave.Entities;
using Hrm.Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Hrm.Infrastructure.Persistence;

/// <summary>
/// ADR-011 W1c — DbContext riêng cho bounded context **LEV** (role <c>hrm_app_lev</c>,
/// <c>ConnectionStrings:LevDbContext</c>). Không sở hữu migration — xem <see cref="PayDbContext"/>.
///
/// LEV **không** ghi <c>EmpAuditLog</c>, nên tách context không tạo giao dịch bắc cầu.
///
/// LEV **có JOIN xuyên schema** sang <c>emp.emp_employee</c>: hàng đợi duyệt C1/C2 lọc theo
/// line manager (<c>LeaveRequestRepository</c>). Đây là truy cập **được phép** theo quyết định
/// ②a — <c>hrm_app_lev</c> có SELECT trên schema <c>emp</c>, EMP là golden record (DOC-11 §4).
/// <see cref="Employees"/> map **chỉ để đọc**; mọi thay đổi hồ sơ đi qua context EMP.
/// </summary>
public class LevDbContext(DbContextOptions<LevDbContext> options) : DbContext(options)
{
    public DbSet<LeaveType> LeaveTypes => Set<LeaveType>();

    public DbSet<LeaveBalance> LeaveBalances => Set<LeaveBalance>();

    public DbSet<LeaveRequest> LeaveRequests => Set<LeaveRequest>();

    public DbSet<LeaveNotification> LeaveNotifications => Set<LeaveNotification>();

    /// <summary>Chỉ đọc — golden record thuộc EMP (②a · DOC-11 §4).</summary>
    public DbSet<Domain.Employees.Entities.Employee> Employees =>
        Set<Domain.Employees.Entities.Employee>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(LevDbContext).Assembly,
            type => type.Namespace == typeof(AppDbContext).Namespace + ".Configurations"
                    && type.Name.StartsWith("Leave", StringComparison.Ordinal));

        // Employee kéo theo cả closure navigation (EducationLevel, Contract, OrgUnit, …).
        // Map thiếu một cái là EF ném "requires a primary key", nên nạp trọn nhóm `emp`
        // — trùng đúng phạm vi mà ②a đã cấp SELECT. Tất cả CHỈ ĐỌC.
        // Đây là cái giá của JOIN xuyên schema: xem ghi chú W3 trong ADR-011.
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(LevDbContext).Assembly,
            type => type.Namespace == typeof(AppDbContext).Namespace + ".Configurations"
                    && (type.Name.StartsWith("Employee", StringComparison.Ordinal)
                        || type.Name.StartsWith("EducationLevel", StringComparison.Ordinal)
                        || type.Name.StartsWith("OrgUnit", StringComparison.Ordinal)
                        || type.Name.StartsWith("SeniorityRule", StringComparison.Ordinal)
                        || type.Name.StartsWith("LineManagerChange", StringComparison.Ordinal)));
    }
}
