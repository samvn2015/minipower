namespace Hrm.Domain.Employees.Repositories;

/// <summary>
/// ADR-011 W1c — audit ghi bằng DbContext của **chính module đó**, để lệnh nghiệp vụ và
/// dòng audit nằm trong **một** transaction (NFR-005 "0 sót").
///
/// Bảng <c>emp_audit_log</c> vẫn dùng chung ở schema <c>shared</c> (quyết định ①a,
/// DEC-DLV-022/024) — mỗi role đã có INSERT sẵn. Chỉ tách *đường vào*, không tách dữ liệu.
/// </summary>
public interface ITimAuditLogRepository : IEmpAuditLogRepository;

/// <inheritdoc cref="ITimAuditLogRepository"/>
public interface IPayAuditLogRepository : IEmpAuditLogRepository;

/// <inheritdoc cref="ITimAuditLogRepository"/>
public interface IPrbAuditLogRepository : IEmpAuditLogRepository;

/// <inheritdoc cref="ITimAuditLogRepository"/>
public interface ILifAuditLogRepository : IEmpAuditLogRepository;
