using Hrm.Domain.Employees;

namespace Hrm.Domain.Employees.Entities;

/// <summary>EMP-SCR-005/006 — đổi LM có duyệt (EMP-FR-008).</summary>
public class LineManagerChangeRequest
{
    public Guid Id { get; set; }

    public Guid EmployeeId { get; set; }

    public Employee Employee { get; set; } = null!;

    public Guid ProposedLineManagerEmployeeId { get; set; }

    public LineManagerChangeStatus Status { get; set; } = LineManagerChangeStatus.Pending;

    public required string RequestedByIdpSubject { get; set; }

    public DateTime RequestedAtUtc { get; set; }

    public string? ReviewedByIdpSubject { get; set; }

    public DateTime? ReviewedAtUtc { get; set; }

    public string? ReviewNote { get; set; }
}
