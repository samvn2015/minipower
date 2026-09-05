using Hrm.Domain.Payroll;

namespace Hrm.Application.Tests.Payroll;

public sealed class PayrollDayCalculatorTests
{
    [Fact]
    public void ComputeNTinh_SubtractsUnpaid_DoesNotAddPaidLeave()
    {
        // N_thực = 22 đã gồm 2 ngày phép hưởng; N_KHL = 1 → N_tính = 21
        var nTinh = PayrollDayCalculator.ComputeNTinh(workDaysIncludingPaidLeave: 22, unpaidLeaveDays: 1);
        Assert.Equal(21m, nTinh);
    }

    [Fact]
    public void ComputeNTinh_ZeroUnpaid_EqualsWorkDays()
    {
        Assert.Equal(20m, PayrollDayCalculator.ComputeNTinh(20, 0));
    }
}
