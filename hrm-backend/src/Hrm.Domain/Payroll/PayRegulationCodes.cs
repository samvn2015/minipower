namespace Hrm.Domain.Payroll;

/// <summary>Mã master quy chế lương.</summary>
public static class PayRegulationCodes
{
    /// <summary>Hệ số lương thời gian khi HĐ thử việc tại kỳ.</summary>
    public const string ProbationTimeWageFactor = "PROBATION_TIME_WAGE_FACTOR";

    /// <summary>Ngày công chuẩn mặc định khi tháng chưa có trên lịch Cty.</summary>
    public const string StandardWorkDaysDefault = "STANDARD_WORK_DAYS_DEFAULT";

    /// <summary>Tỷ lệ BH người lao động hiệu lực kỳ (tổng % master, không hardcode URD).</summary>
    public const string BhEmployeeRate = "BH_EMPLOYEE_RATE";

    /// <summary>Tỷ lệ TNCN tạm hiệu lực kỳ.</summary>
    public const string TncnTempRate = "TNCN_TEMP_RATE";
}
