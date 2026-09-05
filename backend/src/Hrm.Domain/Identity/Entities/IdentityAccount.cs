using Jarvis.Domain.Entities;

namespace Hrm.Domain.Identity.Entities;

/// <summary>DOC-11 IAM · IdentityAccount — map INT-001 / IAM-FR-017.</summary>
public class IdentityAccount : BaseEntity<Guid>
{
    public required string IdpSubject { get; set; }

    public string? EmailCty { get; set; }

    public string? DisplayName { get; set; }

    /// <summary>Mã nhân viên (MNV) — 1:1 map khi có EMP.</summary>
    public string? EmployeeCode { get; set; }

    public IdentityAccountStatus Status { get; set; } = IdentityAccountStatus.Active;

    public ICollection<AccountRole> AccountRoles { get; set; } = [];
}
