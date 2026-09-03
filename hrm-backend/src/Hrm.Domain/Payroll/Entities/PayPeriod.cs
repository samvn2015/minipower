using Jarvis.Domain.Entities;

namespace Hrm.Domain.Payroll.Entities;

/// <summary>Kỳ lương — Draft sau run (PAY-FR-001); Closed chặn bỏ chốt TIM.</summary>
public class PayPeriod : BaseEntity<Guid>
{
    public required string PeriodYm { get; set; }

    public PayPeriodStatus Status { get; set; } = PayPeriodStatus.Draft;

    public DateTime? RanAtUtc { get; set; }

    public string? RanByIdpSubject { get; set; }

    public DateTime? ClosedAtUtc { get; set; }

    public string? ClosedByIdpSubject { get; set; }

    public ICollection<PayLine> Lines { get; set; } = [];
}

/// <summary>Dòng tính kỳ — snapshot từ TIM Closed.</summary>
public class PayLine : BaseEntity<Guid>
{
    public Guid PeriodId { get; set; }

    public PayPeriod? Period { get; set; }

    public Guid EmployeeId { get; set; }

    public required string EmployeeCode { get; set; }

    /// <summary>N_thực (đã gồm phép hưởng).</summary>
    public decimal WorkDays { get; set; }

    /// <summary>N_KHL — phép không hưởng trên TIM.</summary>
    public decimal LeaveDaysUnpaid { get; set; }

    /// <summary>Audit: phép hưởng đã nằm trong WorkDays — không cộng thêm vào N_tính.</summary>
    public decimal LeaveDaysPaid { get; set; }

    /// <summary>N_tính = WorkDays − LeaveDaysUnpaid.</summary>
    public decimal NTinh { get; set; }

    /// <summary>Hệ số lương thời gian (0.85 TV / 1.00 chính thức) — PAY-FR-003.</summary>
    public decimal TimeWageFactor { get; set; } = 1.00m;

    public decimal Ot15 { get; set; }

    public decimal Ot20 { get; set; }

    public decimal Ot30 { get; set; }
}
