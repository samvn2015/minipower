namespace Hrm.Domain.Employees.Entities;

/// <summary>Audit nghiệp vụ EMP — DOC-13 / EMP-TC-NFR-002.</summary>
public class EmpAuditLog
{
    public Guid Id { get; set; }

    public required string Action { get; set; }

    public Guid? EmployeeId { get; set; }

    public Guid? RelatedId { get; set; }

    public required string ActorIdpSubject { get; set; }

    public DateTime OccurredAtUtc { get; set; }

    public string? Detail { get; set; }
}
