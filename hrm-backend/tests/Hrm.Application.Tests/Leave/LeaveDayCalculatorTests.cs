using Hrm.Application.Leave;
using Hrm.Domain.Leave;
using Xunit;

namespace Hrm.Application.Tests.Leave;

public sealed class LeaveDayCalculatorTests
{
    [Fact]
    public void SingleFullDay_ReturnsOne()
    {
        var total = LeaveDayCalculator.Calculate(
            new DateOnly(2026, 9, 1),
            new DateOnly(2026, 9, 1),
            LeaveDayPart.FullDay);
        Assert.Equal(1m, total);
    }

    [Fact]
    public void RangeFullDays_CountsInclusiveDays()
    {
        var total = LeaveDayCalculator.Calculate(
            new DateOnly(2026, 9, 1),
            new DateOnly(2026, 9, 3),
            LeaveDayPart.FullDay);
        Assert.Equal(3m, total);
    }

    [Fact]
    public void HalfDay_ReturnsPointFivePerDay()
    {
        var total = LeaveDayCalculator.Calculate(
            new DateOnly(2026, 9, 1),
            new DateOnly(2026, 9, 2),
            LeaveDayPart.Morning);
        Assert.Equal(1m, total);
    }
}
