using Hrm.Domain.Payroll;

namespace Hrm.Application.Tests.Payroll;

public sealed class PayrollStatutoryCalculatorTests
{
    private static PayrollStatutoryResult ComputeCb(
        decimal e,
        decimal g,
        decimal incomePc,
        decimal meal,
        int dependents,
        decimal advance = 0m,
        decimal factor = 1m,
        decimal f = 26m) =>
        PayrollStatutoryCalculator.Compute(
            e, factor, f, g, incomePc, meal,
            0.08m, 0.015m, 0.01m,
            11_000_000m, dependents, 4_400_000m, advance);

    [Fact]
    public void Compute_Nv001_MatchesCbSheet()
    {
        // I+J+K = 2M+730k+500k
        var r = ComputeCb(25_000_000m, 26m, 3_230_000m, 730_000m, dependents: 1);
        Assert.Equal(25_000_000m, r.TimeWage);
        Assert.Equal(28_230_000m, r.GrossIncome);
        Assert.Equal(2_625_000m, r.BhAmount);
        Assert.Equal(697_500m, r.TncnAmount);
        Assert.Equal(24_907_500m, r.NetPay);
        Assert.Equal(0.105m, r.BhRate);
    }

    [Fact]
    public void Compute_Nv002_MatchesCbSheet()
    {
        var r = ComputeCb(15_000_000m, 25m, 1_030_000m, 730_000m, dependents: 0, advance: 1_000_000m);
        Assert.Equal(14_423_077m, r.TimeWage);
        Assert.Equal(15_453_077m, r.GrossIncome);
        Assert.Equal(1_575_000m, r.BhAmount);
        Assert.Equal(107_403.85m, r.TncnAmount);
        Assert.Equal(12_770_673.15m, r.NetPay);
    }

    [Fact]
    public void Compute_Nv003_MatchesCbSheet()
    {
        var r = ComputeCb(12_000_000m, 26m, 1_030_000m, 730_000m, dependents: 2);
        Assert.Equal(11_770_000m, r.NetPay);
        Assert.Equal(0m, r.TncnAmount);
    }

    [Fact]
    public void Compute_Nv004_MatchesCbSheet()
    {
        var r = ComputeCb(18_000_000m, 24m, 2_030_000m, 730_000m, dependents: 0);
        Assert.Equal(16_615_385m, r.TimeWage);
        Assert.Equal(252_538.5m, r.TncnAmount);
        Assert.Equal(16_502_846.5m, r.NetPay);
    }

    [Fact]
    public void Compute_Nv005_MatchesCbSheet()
    {
        var r = ComputeCb(10_000_000m, 26m, 1_030_000m, 730_000m, dependents: 1);
        Assert.Equal(9_980_000m, r.NetPay);
    }

    [Fact]
    public void Compute_AppliesProbationFactorToTimeWage()
    {
        var r = ComputeCb(10_000_000m, 26m, 0m, 0m, dependents: 0, factor: 0.85m);
        Assert.Equal(8_500_000m, r.TimeWage);
        Assert.Equal(1_050_000m, r.BhAmount); // BH vẫn trên E thỏa thuận
    }

    [Fact]
    public void ProgressiveTncn_BracketBoundaries()
    {
        Assert.Equal(250_000m, PayrollStatutoryCalculator.ComputeProgressiveTncn(5_000_000m));
        Assert.Equal(750_000m, PayrollStatutoryCalculator.ComputeProgressiveTncn(10_000_000m));
    }
}
