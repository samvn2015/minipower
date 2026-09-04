using Jarvis.Domain.Entities;

namespace Hrm.Domain.Lifecycle.Entities;

/// <summary>Case offboarding — PRB-FAIL hoặc LIF UC-002.</summary>
public class LifOffboardingCase : BaseEntity<Guid>
{
    public Guid EmployeeId { get; set; }

    public required string EmployeeCode { get; set; }

    public required string Source { get; set; }

    public LifOffboardingStatus Status { get; set; } = LifOffboardingStatus.Open;

    /// <summary>N = ngày LV cuối — chỉ HR xác nhận (LIF-FR-003).</summary>
    public DateOnly? LastWorkingDayN { get; set; }

    /// <summary>Ngày ký đơn (nếu có) — không được dùng làm N (FR-003).</summary>
    public DateOnly? ResignationSignedDate { get; set; }

    public string? ConfirmedByIdpSubject { get; set; }

    public DateTime? ConfirmedAtUtc { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    public required string CreatedByIdpSubject { get; set; }

    public string? Note { get; set; }

    /// <summary>Khóa Git — luôn cùng lúc CRM SP (LIF-FR-005/006).</summary>
    public DateTime? GitLockedAtUtc { get; set; }

    public DateTime? CrmSpLockedAtUtc { get; set; }

    public DateOnly? LockAsOfDate { get; set; }

    public bool IsEarlySecurityCr { get; set; }

    public string? EarlyCrReason { get; set; }

    public string? LockedByIdpSubject { get; set; }
}
