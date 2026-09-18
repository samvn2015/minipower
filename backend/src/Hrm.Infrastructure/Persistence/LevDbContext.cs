using Hrm.Domain.Leave.Entities;
using Hrm.Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Hrm.Infrastructure.Persistence;

/// <summary>
/// ADR-011 W1c — DbContext riêng cho bounded context **LEV** (role <c>hrm_app_lev</c>,
/// <c>ConnectionStrings:LevDbContext</c>). Không sở hữu migration — xem <see cref="PayDbContext"/>.
///
/// LEV ghi <c>EmpAuditLog</c> (schema <c>shared</c>) cho C1/C2/huỷ phép — bổ sung 2026-09-18
/// (doc-review pass 3 B1, NFR-005). Handler bọc bằng <c>ILevAtomicScope</c> nên nghiệp vụ + outbox
/// + audit là MỘT transaction (pass 4 M1) — PAY/TIM chưa có scope, chưa đạt mức này.
///
/// LEV **có JOIN xuyên schema** sang <c>emp.emp_employee</c>: hàng đợi duyệt C1/C2 lọc theo
/// line manager (<c>LeaveRequestRepository</c>). Đây là truy cập **được phép** theo quyết định
/// ②a — <c>hrm_app_lev</c> có SELECT trên schema <c>emp</c>, EMP là golden record (DOC-11 §4).
/// <see cref="Employees"/> map **chỉ để đọc**; mọi thay đổi hồ sơ đi qua context EMP.
///
/// <para>⚠️ <b>Bẫy cần biết (OQ-ARC-015):</b> từ bản siết quyền, <c>hrm_app_lev</c> chỉ được
/// đọc <b>4 cột</b> của <c>emp.emp_employee</c> (<c>Id</c>, <c>EmployeeCode</c>,
/// <c>FullName</c>, <c>LineManagerEmployeeId</c>) và <b>không</b> có quyền trên
/// <c>emp_contract</c>, <c>emp_education_level</c>, <c>emp_org_unit</c>,
/// <c>emp_seniority_rule</c>, <c>emp_lm_change_request</c> — dù model vẫn map chúng
/// (bắt buộc, vì closure navigation của <c>Employee</c>).
///
/// Nghĩa là: truy vấn nào từ context này <c>Include()</c> hoặc chiếu thêm cột EMP sẽ nhận
/// <c>permission denied</c> lúc chạy. Lỗi **ồn**, không im lặng — nhưng người thêm truy vấn
/// mới cần biết trước. Cần thêm dữ liệu EMP thì gọi qua <c>IEmployeeReadRepository</c>.</para>
/// </summary>
public class LevDbContext(DbContextOptions<LevDbContext> options) : DbContext(options)
{
    public DbSet<LeaveType> LeaveTypes => Set<LeaveType>();

    public DbSet<LeaveBalance> LeaveBalances => Set<LeaveBalance>();

    public DbSet<LeaveRequest> LeaveRequests => Set<LeaveRequest>();

    public DbSet<LeaveNotification> LeaveNotifications => Set<LeaveNotification>();

    /// <summary>Bảng audit dùng chung ở schema <c>shared</c> (①a + phương án A).</summary>
    public DbSet<Domain.Employees.Entities.EmpAuditLog> EmpAuditLogs =>
        Set<Domain.Employees.Entities.EmpAuditLog>();

    /// <summary>Chỉ đọc — golden record thuộc EMP (②a · DOC-11 §4).</summary>
    public DbSet<Domain.Employees.Entities.Employee> Employees =>
        Set<Domain.Employees.Entities.Employee>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(LevDbContext).Assembly,
            type => type.Namespace == typeof(AppDbContext).Namespace + ".Configurations"
                    && (type.Name.StartsWith("Leave", StringComparison.Ordinal)
                        || type.Name.StartsWith("EmpAuditLog", StringComparison.Ordinal)));

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
