namespace Hrm.Domain.Payroll;

/// <summary>Mã quy chế master — C&amp;B / PAY-FR-006/018.</summary>
public static class PayRegulationCodes
{
    public const string ProbationTimeWageFactor = "PROBATION_TIME_WAGE_FACTOR";

    public const string StandardWorkDaysDefault = "STANDARD_WORK_DAYS_DEFAULT";

    /// <summary>Legacy — không dùng tính; giữ seed cũ nếu có.</summary>
    public const string BhEmployeeRate = "BH_EMPLOYEE_RATE";

    public const string BhxhEmployeeRate = "BHXH_EMPLOYEE_RATE";

    public const string BhytEmployeeRate = "BHYT_EMPLOYEE_RATE";

    public const string BhtnEmployeeRate = "BHTN_EMPLOYEE_RATE";

    /// <summary>Legacy flat TNCN — thay bằng lũy tiến C&amp;B.</summary>
    public const string TncnTempRate = "TNCN_TEMP_RATE";

    public const string TncnPersonalDeduction = "TNCN_PERSONAL_DEDUCTION";

    public const string TncnDependentUnit = "TNCN_DEPENDENT_UNIT";
}
