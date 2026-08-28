namespace Hrm.Domain.Identity.Constants;

/// <summary>5 role MVP — IAM DOC-06 / IamSeed.</summary>
public static class IamRoleCodes
{
    public const string Nv = "IAM-ROLE-NV";
    public const string Lm = "IAM-ROLE-LM";
    public const string Hr = "IAM-ROLE-HR";
    public const string It = "IAM-ROLE-IT";
    public const string Pgd = "IAM-ROLE-PGD";

    public static readonly IReadOnlySet<string> All =
        new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            Nv, Lm, Hr, It, Pgd
        };
}
