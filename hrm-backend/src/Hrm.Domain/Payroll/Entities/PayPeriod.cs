using Jarvis.Domain.Entities;

namespace Hrm.Domain.Payroll.Entities;

/// <summary>Kỳ lương stub — dùng chặn bỏ chốt TIM (TIM-FR-012). PAY full tính sau.</summary>
public class PayPeriod : BaseEntity<Guid>
{
    public required string PeriodYm { get; set; }

    public PayPeriodStatus Status { get; set; } = PayPeriodStatus.Draft;

    public DateTime? ClosedAtUtc { get; set; }

    public string? ClosedByIdpSubject { get; set; }
}
