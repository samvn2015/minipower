namespace Hrm.Infrastructure.Persistence.Lev;

internal static class LevSeed
{
    public const string AnnualCode = "LEV-ANNUAL";
    public const string UnpaidCode = "LEV-UNPAID";
    public const string SickCode = "LEV-SICK";
    public const string MarriageCode = "LEV-MARRIAGE";
    public const string BereavementCode = "LEV-BEREAVEMENT";
    public const string MaternityCode = "LEV-MATERNITY";

    public static readonly Guid DevBalance2026Id = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee");

    public const int DevBalanceYear = 2026;

    public const decimal DevEntitledDays = 12m;
}
