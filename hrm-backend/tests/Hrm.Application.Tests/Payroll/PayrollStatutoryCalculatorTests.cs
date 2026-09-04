using Hrm.Domain.Payroll;

namespace Hrm.Application.Tests.Payroll;

public sealed class PayrollStatutoryCalculatorTests
{
    [Fact]
    public void Compute_UsesProvidedRatesNotEmbeddedUrd()
    {
        var a = PayrollStatutoryCalculator.Compute(10_000_000m, 1m, 0m, 0m, 0.10m, 0.05m);
        var b = PayrollStatutoryCalculator.Compute(10_000_000m, 1m, 0m, 0m, 0.08m, 0.01m);

        Assert.Equal(0.10m, a.BhRate);
        Assert.Equal(1_000_000m, a.BhAmount);
        Assert.Equal(450_000m, a.TncnAmount);
        Assert.Equal(8_550_000m, a.NetPay);
        Assert.NotEqual(a.BhAmount, b.BhAmount);
        Assert.NotEqual(a.TncnAmount, b.TncnAmount);
    }

    [Fact]
    public void Compute_AppliesTimeWageFactorToSalary()
    {
        var result = PayrollStatutoryCalculator.Compute(10_000_000m, 0.85m, 0m, 0m, 0.10m, 0.05m);
        Assert.Equal(850_000m, result.BhAmount);
    }
}
