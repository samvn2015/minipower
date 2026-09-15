using Hrm.Domain.Employees.Repositories;

namespace Hrm.Infrastructure.Persistence.Repositories;

// ADR-011 W1c phương án A — mỗi bounded context ghi audit bằng DbContext của mình,
// giữ lệnh nghiệp vụ và dòng audit trong một transaction. Logic ở EmpAuditLogRepositoryBase.

internal sealed class EmpAuditLogRepository(EmpDbContext db)
    : EmpAuditLogRepositoryBase(db), IEmpAuditLogRepository;

internal sealed class TimAuditLogRepository(TimDbContext db)
    : EmpAuditLogRepositoryBase(db), ITimAuditLogRepository;

internal sealed class PayAuditLogRepository(PayDbContext db)
    : EmpAuditLogRepositoryBase(db), IPayAuditLogRepository;

internal sealed class PrbAuditLogRepository(PrbDbContext db)
    : EmpAuditLogRepositoryBase(db), IPrbAuditLogRepository;

internal sealed class LifAuditLogRepository(LifDbContext db)
    : EmpAuditLogRepositoryBase(db), ILifAuditLogRepository;
