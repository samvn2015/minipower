namespace Hrm.Domain.Payroll;

/// <summary>
/// Tạm tính BH + TNCN từ tỷ lệ master kỳ — PAY-FR-006.
/// Không chứa % cố định; caller truyền DecimalValue từ pay_regulation.
/// </summary>
public static class PayrollStatutoryCalculator
{
    public static PayrollStatutoryResult Compute(
        decimal contractSalary,
        decimal timeWageFactor,
        decimal contractAllowance,
        decimal monthlyAllowance,
        decimal bhEmployeeRate,
        decimal tncnTempRate)
    {
        var timeWage = contractSalary * timeWageFactor;
        var gross = timeWage + contractAllowance + monthlyAllowance;
        var bh = RoundMoney(gross * bhEmployeeRate);
        var tncnBase = gross - bh;
        if (tncnBase < 0m)
            tncnBase = 0m;
        var tncn = RoundMoney(tncnBase * tncnTempRate);
        var net = gross - bh - tncn;
        return new PayrollStatutoryResult(bhEmployeeRate, tncnTempRate, bh, tncn, net);
    }

    private static decimal RoundMoney(decimal value) =>
        decimal.Round(value, 2, MidpointRounding.AwayFromZero);
}

public sealed record PayrollStatutoryResult(
    decimal BhRate,
    decimal TncnRate,
    decimal BhAmount,
    decimal TncnAmount,
    decimal NetPay);
