namespace Hrm.Domain.Identity.Entities;

/// <summary>DOC-11 IAM · Role — 5 role MVP (IAM DOC-06).</summary>
public class Role
{
    public required string RoleCode { get; set; }

    public required string Name { get; set; }

    public ICollection<AccountRole> AccountRoles { get; set; } = [];
}
