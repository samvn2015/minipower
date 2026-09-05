namespace Hrm.Domain.Leave;

/// <summary>LEV-FR-003 — overlap đơn Open (PendingC1 / PendingC2 / Approved).</summary>
public static class LeaveOverlapChecker
{
    public static bool IsOpenStatus(LeaveRequestStatus status) =>
        status is LeaveRequestStatus.PendingC1
            or LeaveRequestStatus.PendingC2
            or LeaveRequestStatus.Approved;

    public static bool DayPartsConflict(LeaveDayPart left, LeaveDayPart right) =>
        left == LeaveDayPart.FullDay || right == LeaveDayPart.FullDay || left == right;

    public static bool Overlaps(
        DateOnly fromA,
        DateOnly toA,
        LeaveDayPart partA,
        DateOnly fromB,
        DateOnly toB,
        LeaveDayPart partB)
    {
        if (toA < fromB || toB < fromA)
            return false;

        return DayPartsConflict(partA, partB);
    }
}
