namespace Hrm.Domain.Leave;

/// <summary>
/// Hạn nộp 3 NLĐ khi ≥3 ngày công chuẩn liền (LEV-FR-006 / BR-007).
/// MVP: ngày công = T2–T6 (chưa gắn lịch lễ Cty — ghi nợ nếu cần).
/// </summary>
public static class LeaveAdvanceNotice
{
    public static bool IsStandardWorkDay(DateOnly date) =>
        date.DayOfWeek is not (DayOfWeek.Saturday or DayOfWeek.Sunday);

    /// <summary>
    /// Số ngày công trong khoảng nghỉ liền (CN/lễ không đếm, không phá chuỗi công).
    /// </summary>
    public static int CountConsecutiveWorkDaysInRange(DateOnly fromDate, DateOnly toDate)
    {
        if (toDate < fromDate)
            return 0;

        var count = 0;
        for (var d = fromDate; d <= toDate; d = d.AddDays(1))
        {
            if (IsStandardWorkDay(d))
                count++;
        }

        return count;
    }

    /// <summary>Số NLĐ giữa ngày nộp (không tính) và ngày bắt đầu (không tính).</summary>
    public static int CountBusinessDaysBeforeStart(DateOnly submittedOn, DateOnly leaveStart)
    {
        if (leaveStart <= submittedOn)
            return 0;

        var count = 0;
        for (var d = submittedOn.AddDays(1); d < leaveStart; d = d.AddDays(1))
        {
            if (IsStandardWorkDay(d))
                count++;
        }

        return count;
    }

    public static bool RequiresAdvanceNotice(DateOnly fromDate, DateOnly toDate) =>
        CountConsecutiveWorkDaysInRange(fromDate, toDate) >= 3;

    public static bool IsLateWithoutEmergency(
        DateOnly submittedOn,
        DateOnly fromDate,
        DateOnly toDate,
        bool isEmergency)
    {
        if (isEmergency)
            return false;
        if (!RequiresAdvanceNotice(fromDate, toDate))
            return false;
        return CountBusinessDaysBeforeStart(submittedOn, fromDate) < 3;
    }
}
