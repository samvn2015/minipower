using Jarvis.Domain.Entities;

namespace Hrm.Domain.Payroll.Entities;

/// <summary>Lương HĐ (cơ sở tạm BH/TNCN) — C&amp;B master PAY, không hardcode URD.</summary>
public class PayContractSalary : BaseEntity<Guid>
{
    public Guid EmployeeId { get; set; }

    public required string EmployeeCode { get; set; }

    public decimal Amount { get; set; }

    /// <summary>Số người phụ thuộc TNCN — C&amp;B.</summary>
    public int DependentCount { get; set; }
}
