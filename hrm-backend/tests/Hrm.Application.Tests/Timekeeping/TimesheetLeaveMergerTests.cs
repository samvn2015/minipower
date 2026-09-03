using Hrm.Domain.Timekeeping;
using Hrm.Domain.Timekeeping.Repositories;

namespace Hrm.Application.Tests.Timekeeping;

public sealed class TimesheetLeaveMergerTests
{
    private static readonly Guid Emp = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");

    [Fact]
    public void BuildMergeLines_ApprovedAnnual_PaidDays()
    {
        var lines = TimesheetLeaveMerger.BuildMergeLines(
            "2026-11",
            [Emp],
            [
                new ApprovedLeaveInput(
                    Emp,
                    "LEV-ANNUAL",
                    DeductsAnnualBalance: true,
                    new DateOnly(2026, 11, 3),
                    new DateOnly(2026, 11, 4),
                    2m)
            ]);

        Assert.Single(lines);
        Assert.Equal(2m, lines[0].LeaveDaysPaid);
        Assert.Equal(0m, lines[0].LeaveDaysUnpaid);
    }

    [Fact]
    public void BuildMergeLines_Unpaid_GoesToUnpaidBucket()
    {
        var lines = TimesheetLeaveMerger.BuildMergeLines(
            "2026-11",
            [Emp],
            [
                new ApprovedLeaveInput(
                    Emp,
                    "LEV-UNPAID",
                    DeductsAnnualBalance: false,
                    new DateOnly(2026, 11, 5),
                    new DateOnly(2026, 11, 5),
                    1m)
            ]);

        Assert.Equal(1m, lines[0].LeaveDaysUnpaid);
        Assert.Equal(0m, lines[0].LeaveDaysPaid);
    }

    [Fact]
    public void OverlapDays_OutsideMonth_Zero()
    {
        var days = TimesheetLeaveClassification.OverlapDaysInMonth(
            new DateOnly(2026, 10, 1),
            new DateOnly(2026, 10, 5),
            5m,
            "2026-11");
        Assert.Equal(0m, days);
    }
}
