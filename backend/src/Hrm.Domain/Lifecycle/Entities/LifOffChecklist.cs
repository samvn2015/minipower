using Jarvis.Domain.Entities;

namespace Hrm.Domain.Lifecycle.Entities;

/// <summary>Master checklist off — LIF-FR-009 (IsMust chặn đóng).</summary>
public class LifOffChecklistItem : BaseEntity<Guid>
{
    public required string Code { get; set; }

    public required string Name { get; set; }

    public bool IsMust { get; set; }

    public bool IsActive { get; set; } = true;

    public int SortOrder { get; set; }
}

/// <summary>Tick checklist trên case offboarding.</summary>
public class LifOffChecklistTick : BaseEntity<Guid>
{
    public Guid OffboardingCaseId { get; set; }

    public required string ItemCode { get; set; }

    public bool IsChecked { get; set; }

    public string? CheckedByIdpSubject { get; set; }

    public DateTime? CheckedAtUtc { get; set; }
}
