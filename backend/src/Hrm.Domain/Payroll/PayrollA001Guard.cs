namespace Hrm.Domain.Payroll;

/// <summary>
/// A-001: TIM tách phép hưởng khỏi N_thực — PAY không im lặng cộng lại, không tự sửa công (PAY-FR-013).
/// </summary>
public static class PayrollA001Guard
{
    public const string Code = "A-001";

    /// <summary>
    /// N_tính không bao giờ cộng LeaveDaysPaid — chỉ N_thực − N_KHL.
    /// </summary>
    public static decimal ComputeNTinh(
        decimal workDays,
        decimal leaveDaysUnpaid,
        decimal leaveDaysPaid)
    {
        _ = leaveDaysPaid; // intentional: không cộng phép hưởng (PAY-FR-002/013)
        return PayrollDayCalculator.ComputeNTinh(workDays, leaveDaysUnpaid);
    }

    public static string? BuildWarning(string employeeCode, decimal leaveDaysPaid)
    {
        if (leaveDaysPaid <= 0)
            return null;

        return
            $"{Code}: {employeeCode} — TIM báo phép hưởng tách cột; PAY không cộng vào N_tính và không tự sửa công (PAY-FR-013).";
    }

    public static IReadOnlyList<string> CollectWarnings(
        IEnumerable<(string EmployeeCode, decimal LeaveDaysPaid)> lines)
    {
        return lines
            .Select(l => BuildWarning(l.EmployeeCode, l.LeaveDaysPaid))
            .Where(static w => w is not null)
            .Cast<string>()
            .Distinct(StringComparer.Ordinal)
            .ToList();
    }
}
