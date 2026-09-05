using Jarvis.Domain.Entities;

namespace Hrm.Domain.Payroll.Entities;

/// <summary>Master quy chế PAY — tỷ lệ TV v.v. (PAY-FR-003: đổi master, không sửa URD).</summary>
public class PayRegulation : BaseEntity<Guid>
{
    public required string Code { get; set; }

    public required string Name { get; set; }

    public decimal DecimalValue { get; set; }
}
