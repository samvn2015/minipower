namespace Hrm.Infrastructure.Persistence.Iam;

/// <summary>Seed IAM MVP — IAM DOC-06 / DOC-11.</summary>
internal static class IamSeed
{
    public static readonly Guid DevAccountId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");

    public static readonly Guid ItDevAccountId = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc");

    /// <summary>Sub mặc định local khi test Swagger không có JWT thật.</summary>
    public const string DevIdpSubject = "local-dev";

    public const string ItDevIdpSubject = "it-dev";

    public static class Roles
    {
        public const string Nv = "IAM-ROLE-NV";
        public const string Lm = "IAM-ROLE-LM";
        public const string Hr = "IAM-ROLE-HR";
        public const string It = "IAM-ROLE-IT";
        public const string Pgd = "IAM-ROLE-PGD";
    }
}
