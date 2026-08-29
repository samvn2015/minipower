using Hrm.Domain.Leave;

namespace Hrm.Application.Leave;

/// <summary>LEV-BR-002 — tính tổng ngày đơn (MVP: cùng nhãn buổi cho mọi ngày trong khoảng).</summary>
public static class LeaveDayCalculator
{
    public static decimal Calculate(DateOnly fromDate, DateOnly toDate, LeaveDayPart dayPart)
    {
        if (toDate < fromDate)
            throw new ArgumentException("ToDate phải >= FromDate.");

        var daysInRange = toDate.DayNumber - fromDate.DayNumber + 1;
        var unit = dayPart switch
        {
            LeaveDayPart.FullDay => 1.0m,
            LeaveDayPart.Morning => 0.5m,
            LeaveDayPart.Afternoon => 0.5m,
            _ => throw new ArgumentOutOfRangeException(nameof(dayPart))
        };

        return daysInRange * unit;
    }
}
