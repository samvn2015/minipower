using Jarvis.Domain.Entities;

namespace Hrm.Domain.Payroll.Entities;

/// <summary>PC/thưởng kênh HĐ cố định — PAY-FR-005 kênh 1.</summary>
public class PayContractAllowance : BaseEntity<Guid>
{
    public Guid EmployeeId { get; set; }

    public required string EmployeeCode { get; set; }

    public required string Code { get; set; }

    public decimal Amount { get; set; }
}
