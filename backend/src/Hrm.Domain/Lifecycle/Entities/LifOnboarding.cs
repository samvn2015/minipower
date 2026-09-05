using Jarvis.Domain.Entities;

namespace Hrm.Domain.Lifecycle.Entities;

/// <summary>Case onboarding — LIF-UC-001 / FR-001/002.</summary>
public class LifOnboardingCase : BaseEntity<Guid>
{
    public Guid EmployeeId { get; set; }

    public required string EmployeeCode { get; set; }

    public LifOnboardingStatus Status { get; set; } = LifOnboardingStatus.Open;

    public DateTime CreatedAtUtc { get; set; }

    public required string CreatedByIdpSubject { get; set; }

    public string? Note { get; set; }

    public bool EmailCtyProvisioned { get; set; }

    public bool GitProvisioned { get; set; }

    public bool CrmSpProvisioned { get; set; }

    public bool ChatProvisioned { get; set; }

    public DateTime? EmailCtyProvisionedAtUtc { get; set; }

    public DateTime? GitProvisionedAtUtc { get; set; }

    public DateTime? CrmSpProvisionedAtUtc { get; set; }

    public DateTime? ChatProvisionedAtUtc { get; set; }

    public string? ClosedByIdpSubject { get; set; }

    public DateTime? ClosedAtUtc { get; set; }
}

/// <summary>Master checklist on — LIF-FR-001.</summary>
public class LifOnChecklistItem : BaseEntity<Guid>
{
    public required string Code { get; set; }

    public required string Name { get; set; }

    public bool IsMust { get; set; }

    public bool IsActive { get; set; } = true;

    public int SortOrder { get; set; }
}

public class LifOnChecklistTick : BaseEntity<Guid>
{
    public Guid OnboardingCaseId { get; set; }

    public required string ItemCode { get; set; }

    public bool IsChecked { get; set; }

    public string? CheckedByIdpSubject { get; set; }

    public DateTime? CheckedAtUtc { get; set; }
}
