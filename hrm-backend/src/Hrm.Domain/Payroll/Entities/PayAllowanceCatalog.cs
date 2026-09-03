using Jarvis.Domain.Entities;

namespace Hrm.Domain.Payroll.Entities;

/// <summary>Master mã PC/thưởng kỳ — PAY-FR-005/015.</summary>
public class PayAllowanceCatalog : BaseEntity<Guid>
{
    public required string Code { get; set; }

    public required string Name { get; set; }

    public bool IsActive { get; set; } = true;
}
