using Jarvis.Domain.Entities;

namespace Hrm.Domain.Payroll.Entities;

/// <summary>PC/thưởng nhập tháng — PAY-FR-015 · PAY-UC-003.</summary>
public class PayMonthlyAllowance : BaseEntity<Guid>
{
    public required string PeriodYm { get; set; }

    public Guid EmployeeId { get; set; }

    public required string EmployeeCode { get; set; }

    public required string Code { get; set; }

    public decimal Amount { get; set; }
}
