namespace Hrm.Domain.Identity.Entities;

/// <summary>DOC-11 IAM · AccountRole.</summary>
public class AccountRole
{
    public Guid AccountId { get; set; }

    public required string RoleCode { get; set; }

    public IdentityAccount Account { get; set; } = null!;

    public Role Role { get; set; } = null!;
}
