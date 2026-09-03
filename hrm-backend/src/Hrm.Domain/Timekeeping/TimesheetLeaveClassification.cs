namespace Hrm.Domain.Timekeeping;

/// <summary>Phân loại phép cho bảng công — TIM-FR-008/009 · TIM-BR-007/008.</summary>
public static class TimesheetLeaveClassification
{
    public const string UnpaidCode = "LEV-UNPAID";
    public const string MarriageCode = "LEV-MARRIAGE";
    public const string BereavementCode = "LEV-BEREAVEMENT";

    public enum Kind
    {
        Paid,
        Unpaid,
        Other
    }

    public static Kind Classify(string leaveTypeCode, bool deductsAnnualBalance)
    {
        if (string.Equals(leaveTypeCode, UnpaidCode, StringComparison.OrdinalIgnoreCase))
            return Kind.Unpaid;

        if (deductsAnnualBalance
            || string.Equals(leaveTypeCode, MarriageCode, StringComparison.OrdinalIgnoreCase)
            || string.Equals(leaveTypeCode, BereavementCode, StringComparison.OrdinalIgnoreCase))
        {
            return Kind.Paid;
        }

        return Kind.Other;
    }

    /// <summary>Số ngày phép giao với tháng YYYY-MM (tỷ lệ theo TotalDays).</summary>
    public static decimal OverlapDaysInMonth(
        DateOnly fromDate,
        DateOnly toDate,
        decimal totalDays,
        string periodYm)
    {
        if (periodYm.Length != 7 || periodYm[4] != '-')
            return 0;

        if (!int.TryParse(periodYm.AsSpan(0, 4), out var year)
            || !int.TryParse(periodYm.AsSpan(5, 2), out var month)
            || month is < 1 or > 12)
        {
            return 0;
        }

        var monthStart = new DateOnly(year, month, 1);
        var monthEnd = monthStart.AddMonths(1).AddDays(-1);
        var overlapStart = fromDate > monthStart ? fromDate : monthStart;
        var overlapEnd = toDate < monthEnd ? toDate : monthEnd;
        if (overlapStart > overlapEnd)
            return 0;

        var leaveSpan = toDate.DayNumber - fromDate.DayNumber + 1;
        if (leaveSpan <= 0)
            return 0;

        var overlapSpan = overlapEnd.DayNumber - overlapStart.DayNumber + 1;
        var days = totalDays * overlapSpan / leaveSpan;
        return Math.Round(days, 2, MidpointRounding.AwayFromZero);
    }
}
