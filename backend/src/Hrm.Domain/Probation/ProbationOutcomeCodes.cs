namespace Hrm.Domain.Probation;

/// <summary>3 mã kết quả SoT — PRB-FR-004 (không mã ngoài master).</summary>
public static class ProbationOutcomeCodes
{
    public const string Pass = "PASS";
    public const string Extend = "EXTEND";
    public const string Fail = "FAIL";

    public static readonly IReadOnlySet<string> All =
        new HashSet<string>(StringComparer.OrdinalIgnoreCase) { Pass, Extend, Fail };
}
