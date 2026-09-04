namespace Hrm.Domain.Lifecycle;

/// <summary>N+3 ngày lịch từ N (LIF-FR-005 nháp · FR-013 hiển thị).</summary>
public static class LifOffboardingFacts
{
    public const int NPlus3CalendarDays = 3;

    public static DateOnly ComputeNPlus3(DateOnly lastWorkingDayN) =>
        lastWorkingDayN.AddDays(NPlus3CalendarDays);
}
