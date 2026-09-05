using Hrm.Domain.Payroll;

namespace Hrm.Application.Tests.Payroll;

public sealed class PayrollA001GuardTests
{
    [Fact]
    public void ComputeNTinh_IgnoresPaidLeave()
    {
        // workDays=20 đã không gồm phép hưởng; paid=2 — không cộng thêm.
        Assert.Equal(20m, PayrollA001Guard.ComputeNTinh(20m, 0m, 2m));
        Assert.Equal(18m, PayrollA001Guard.ComputeNTinh(20m, 2m, 3m));
    }

    [Fact]
    public void BuildWarning_WhenPaidLeave_ReturnsA001()
    {
        var w = PayrollA001Guard.BuildWarning("MNV-DEV", 2m);
        Assert.NotNull(w);
        Assert.Contains("A-001", w, StringComparison.Ordinal);
        Assert.Contains("PAY-FR-013", w, StringComparison.Ordinal);
    }

    [Fact]
    public void BuildWarning_WhenNoPaidLeave_Null()
    {
        Assert.Null(PayrollA001Guard.BuildWarning("MNV-DEV", 0m));
    }
}
