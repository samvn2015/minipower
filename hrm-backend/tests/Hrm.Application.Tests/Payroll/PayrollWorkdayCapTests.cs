using Hrm.Domain.Payroll;

namespace Hrm.Application.Tests.Payroll;

public sealed class PayrollWorkdayCapTests
{
    [Theory]
    [InlineData(23, 21, true)]
    [InlineData(21, 21, false)]
    [InlineData(20, 22, false)]
    public void ExceedsCap_ComparesStrictlyGreater(decimal nTinh, decimal standard, bool expected) =>
        Assert.Equal(expected, PayrollWorkdayCap.ExceedsCap(nTinh, standard));
}
