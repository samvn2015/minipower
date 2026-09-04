namespace Hrm.Domain.Lifecycle;

/// <summary>N+3 ngày lịch từ N (LIF-FR-005 nháp · FR-013 hiển thị).</summary>
public static class LifOffboardingFacts
{
    public const int NPlus3CalendarDays = 3;

    public const string LockTargetSystems = "Git;CrmSp";

    /// <summary>Connector channel — không CRM sales (LIF-FR-010).</summary>
    public const string LockChannelGitAndCrmSp = "git+crm-sp";

    public static DateOnly ComputeNPlus3(DateOnly lastWorkingDayN) =>
        lastWorkingDayN.AddDays(NPlus3CalendarDays);

    public static bool IsNPlus3Reached(DateOnly lastWorkingDayN, DateOnly asOf) =>
        asOf >= ComputeNPlus3(lastWorkingDayN);
}
