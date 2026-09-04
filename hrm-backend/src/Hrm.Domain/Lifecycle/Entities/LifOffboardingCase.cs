using Jarvis.Domain.Entities;

namespace Hrm.Domain.Lifecycle.Entities;

/// <summary>Case offboarding — mở từ PRB Không đạt (PRB-FR-007) hoặc LIF UC-002.</summary>
public class LifOffboardingCase : BaseEntity<Guid>
{
    public Guid EmployeeId { get; set; }

    public required string EmployeeCode { get; set; }

    public required string Source { get; set; }

    public LifOffboardingStatus Status { get; set; } = LifOffboardingStatus.Open;

    /// <summary>N = ngày LV cuối — chỉ HR xác nhận (LIF-FR-003); null khi mới mở từ PRB.</summary>
    public DateOnly? LastWorkingDayN { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    public required string CreatedByIdpSubject { get; set; }

    public string? Note { get; set; }
}
