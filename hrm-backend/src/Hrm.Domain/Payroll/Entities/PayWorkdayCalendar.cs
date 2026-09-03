using Jarvis.Domain.Entities;

namespace Hrm.Domain.Payroll.Entities;

/// <summary>Ngày công chuẩn tháng — lịch Cty D-004 (PAY-FR-007).</summary>
public class PayWorkdayCalendar : BaseEntity<Guid>
{
    public required string PeriodYm { get; set; }

    public decimal StandardWorkDays { get; set; }
}
