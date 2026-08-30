using Jarvis.Domain.Entities;

namespace Hrm.Domain.Leave.Entities;

/// <summary>Quỹ phép năm theo NV — LEV-FR-015.</summary>
public class LeaveBalance : BaseEntity<Guid>
{
    public Guid EmployeeId { get; set; }

    public int Year { get; set; }

    public decimal EntitledDays { get; set; }

    public decimal UsedDays { get; set; }

    public decimal RemainingDays => EntitledDays - UsedDays;
}
