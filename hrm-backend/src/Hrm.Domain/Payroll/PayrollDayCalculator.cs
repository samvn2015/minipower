namespace Hrm.Domain.Payroll;

/// <summary>N_tính = N_thực − N_KHL; không cộng phép hưởng (PAY-FR-002 · PAY-BR-001).</summary>
public static class PayrollDayCalculator
{
    public static decimal ComputeNTinh(decimal workDaysIncludingPaidLeave, decimal unpaidLeaveDays)
    {
        var nTinh = workDaysIncludingPaidLeave - unpaidLeaveDays;
        return Math.Round(nTinh, 2, MidpointRounding.AwayFromZero);
    }
}
