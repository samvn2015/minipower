namespace Hrm.Domain.Employees.Repositories;

public sealed record EmpAuditLogEntry(
    string Action,
    Guid? EmployeeId,
    Guid? RelatedId,
    string ActorIdpSubject,
    string? Detail);

public sealed record EmpAuditLogSnapshot(
    Guid Id,
    string Action,
    Guid? EmployeeId,
    Guid? RelatedId,
    string ActorIdpSubject,
    DateTime OccurredAtUtc,
    string? Detail);

public interface IEmpAuditLogRepository
{
    Task AppendAsync(EmpAuditLogEntry entry, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<EmpAuditLogSnapshot>> ListByEmployeeIdAsync(
        Guid employeeId,
        CancellationToken cancellationToken = default);
}
