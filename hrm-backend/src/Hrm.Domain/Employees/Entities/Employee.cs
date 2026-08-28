using Jarvis.Domain.Entities;

namespace Hrm.Domain.Employees.Entities;

/// <summary>DOC-11 Employee — khung EMP skeleton.</summary>
public class Employee : BaseEntity<Guid>
{
    /// <summary>Mã nhân viên (MNV) — unique.</summary>
    public required string EmployeeCode { get; set; }

    public string? FullName { get; set; }

    public string? Cccd { get; set; }

    public string? EmailCty { get; set; }

    /// <summary>Mã số thuế (MST).</summary>
    public string? TaxId { get; set; }

    public Guid? LineManagerEmployeeId { get; set; }

    public EmployeeStatus Status { get; set; } = EmployeeStatus.Active;
}
