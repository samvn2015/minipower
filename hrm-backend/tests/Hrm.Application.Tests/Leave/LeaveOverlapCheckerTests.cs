using Hrm.Domain.Leave;

namespace Hrm.Application.Tests.Leave;

public sealed class LeaveOverlapCheckerTests
{
    [Theory]
    [InlineData("2026-01-10", "2026-01-10", LeaveDayPart.FullDay, "2026-01-10", "2026-01-10", LeaveDayPart.Morning, true)]
    [InlineData("2026-01-10", "2026-01-10", LeaveDayPart.Morning, "2026-01-10", "2026-01-10", LeaveDayPart.Afternoon, false)]
    [InlineData("2026-01-10", "2026-01-12", LeaveDayPart.FullDay, "2026-01-13", "2026-01-14", LeaveDayPart.FullDay, false)]
    public void Overlaps_VariousCases(
        string fromA,
        string toA,
        LeaveDayPart partA,
        string fromB,
        string toB,
        LeaveDayPart partB,
        bool expected)
    {
        var result = LeaveOverlapChecker.Overlaps(
            DateOnly.Parse(fromA),
            DateOnly.Parse(toA),
            partA,
            DateOnly.Parse(fromB),
            DateOnly.Parse(toB),
            partB);

        Assert.Equal(expected, result);
    }
}
