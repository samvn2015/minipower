using Hrm.Domain.Leave;

namespace Hrm.Application.Tests.Leave;

public sealed class LeaveAdvanceNoticeTests
{
    [Fact]
    public void CountConsecutiveWorkDays_FriToMon_IsTwo()
    {
        // 2026-10-02 Fri … 2026-10-05 Mon
        var n = LeaveAdvanceNotice.CountConsecutiveWorkDaysInRange(
            new DateOnly(2026, 10, 2),
            new DateOnly(2026, 10, 5));
        Assert.Equal(2, n);
        Assert.False(LeaveAdvanceNotice.RequiresAdvanceNotice(
            new DateOnly(2026, 10, 2),
            new DateOnly(2026, 10, 5)));
    }

    [Fact]
    public void CountConsecutiveWorkDays_WedToFri_IsThree()
    {
        var from = new DateOnly(2026, 10, 7); // Wed
        var to = new DateOnly(2026, 10, 9); // Fri
        Assert.Equal(3, LeaveAdvanceNotice.CountConsecutiveWorkDaysInRange(from, to));
        Assert.True(LeaveAdvanceNotice.RequiresAdvanceNotice(from, to));
    }

    [Fact]
    public void IsLateWithoutEmergency_BlocksWhenFewerThan3BusinessDays()
    {
        var from = new DateOnly(2026, 10, 7);
        var to = new DateOnly(2026, 10, 9);
        var submitted = new DateOnly(2026, 10, 5); // Mon → Tue only before Wed = 1 NLĐ
        Assert.True(LeaveAdvanceNotice.IsLateWithoutEmergency(submitted, from, to, isEmergency: false));
        Assert.False(LeaveAdvanceNotice.IsLateWithoutEmergency(submitted, from, to, isEmergency: true));
    }

    [Fact]
    public void IsLateWithoutEmergency_AllowsWhenEnoughNotice()
    {
        var from = new DateOnly(2026, 10, 12); // Mon
        var to = new DateOnly(2026, 10, 14); // Wed
        var submitted = new DateOnly(2026, 10, 5); // prior Mon → Tue,Wed,Thu,Fri = 4
        Assert.False(LeaveAdvanceNotice.IsLateWithoutEmergency(submitted, from, to, isEmergency: false));
    }
}
