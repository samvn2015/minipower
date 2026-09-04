namespace Hrm.Domain.Payroll;

/// <summary>
/// Tính BH/TNCN/thực lĩnh theo quy chế bảng C&amp;B (PAY-FR-006/018).
/// BH trên lương thỏa thuận E; H = ROUND(E×factor/F×G,0); TNCN lũy tiến.
/// </summary>
public static class PayrollStatutoryCalculator
{
    public static PayrollStatutoryResult Compute(
        decimal contractSalary,
        decimal timeWageFactor,
        decimal standardWorkDays,
        decimal nTinh,
        decimal incomeAllowances,
        decimal mealTaxExempt,
        decimal bhxhRate,
        decimal bhytRate,
        decimal bhtnRate,
        decimal personalDeduction,
        int dependentCount,
        decimal dependentUnitAmount,
        decimal advanceAmount)
    {
        if (standardWorkDays <= 0m)
            throw new ArgumentOutOfRangeException(nameof(standardWorkDays));

        var factor = timeWageFactor <= 0m ? 1m : timeWageFactor;
        var H = RoundDong(contractSalary * factor / standardWorkDays * nTinh);
        var L = H + incomeAllowances;

        var M = RoundDong(contractSalary * bhxhRate);
        var N = RoundDong(contractSalary * bhytRate);
        var O = RoundDong(contractSalary * bhtnRate);
        var P = M + N + O;

        var dependents = dependentCount < 0 ? 0 : dependentCount;
        var S = dependents * dependentUnitAmount;
        var exemptMeal = mealTaxExempt < 0m ? 0m : mealTaxExempt;
        var T = L - exemptMeal - P - personalDeduction - S;
        if (T < 0m)
            T = 0m;

        var U = ComputeProgressiveTncn(T);
        var V = advanceAmount < 0m ? 0m : advanceAmount;
        var W = L - P - U - V;

        var bhRate = bhxhRate + bhytRate + bhtnRate;
        var tncnRate = T > 0m ? decimal.Round(U / T, 6, MidpointRounding.AwayFromZero) : 0m;

        return new PayrollStatutoryResult(bhRate, tncnRate, P, U, W, H, L);
    }

    /// <summary>Biểu TNCN lũy tiến — khớp công thức sheet C&amp;B (chưa ROUND bước U).</summary>
    public static decimal ComputeProgressiveTncn(decimal taxableIncome)
    {
        if (taxableIncome <= 0m)
            return 0m;
        if (taxableIncome <= 5_000_000m)
            return taxableIncome * 0.05m;
        if (taxableIncome <= 10_000_000m)
            return taxableIncome * 0.10m - 250_000m;
        if (taxableIncome <= 18_000_000m)
            return taxableIncome * 0.15m - 750_000m;
        return taxableIncome * 0.20m - 1_650_000m;
    }

    public static decimal RoundDong(decimal value) =>
        decimal.Round(value, 0, MidpointRounding.AwayFromZero);
}

public sealed record PayrollStatutoryResult(
    decimal BhRate,
    decimal TncnRate,
    decimal BhAmount,
    decimal TncnAmount,
    decimal NetPay,
    decimal TimeWage = 0m,
    decimal GrossIncome = 0m);
