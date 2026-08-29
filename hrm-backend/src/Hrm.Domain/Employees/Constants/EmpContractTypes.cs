namespace Hrm.Domain.Employees.Constants;

/// <summary>Master loại HĐ — EMP-FR-014 (seed; không hardcode UI).</summary>
public static class EmpContractTypes
{
    public const string Probation = "PROBATION";
    public const string Official = "OFFICIAL";

    public static readonly IReadOnlySet<string> All =
        new HashSet<string>(StringComparer.OrdinalIgnoreCase) { Probation, Official };
}
