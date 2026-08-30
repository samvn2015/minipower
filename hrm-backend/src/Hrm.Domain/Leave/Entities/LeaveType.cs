namespace Hrm.Domain.Leave.Entities;

/// <summary>Catalog loại phép — LEV-BR-001.</summary>
public class LeaveType
{
    public required string Code { get; set; }

    public required string Name { get; set; }

    /// <summary>Loại này trừ quỹ phép năm khi HR duyệt C2 (LEV-BR-005/006).</summary>
    public bool DeductsAnnualBalance { get; set; }

    public LeaveTypeStatus Status { get; set; } = LeaveTypeStatus.Active;
}
