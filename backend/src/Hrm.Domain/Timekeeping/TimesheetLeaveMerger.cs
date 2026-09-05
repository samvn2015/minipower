using Hrm.Domain.Timekeeping.Repositories;

namespace Hrm.Domain.Timekeeping;

/// <summary>Input gọn cho merger (tránh phụ thuộc Leave entity).</summary>
public sealed record ApprovedLeaveInput(
    Guid EmployeeId,
    string LeaveTypeCode,
    bool DeductsAnnualBalance,
    DateOnly FromDate,
    DateOnly ToDate,
    decimal TotalDays);

/// <summary>Gộp phép Approved theo NV cho một kỳ công — TIM-FR-008/009.</summary>
public static class TimesheetLeaveMerger
{
    public static IReadOnlyList<TimesheetLeaveMergeLine> BuildMergeLines(
        string periodYm,
        IReadOnlyList<Guid> employeeIds,
        IReadOnlyList<ApprovedLeaveInput> leaves)
    {
        var byEmployee = employeeIds.ToDictionary(
            id => id,
            _ => new Acc());

        foreach (var leave in leaves)
        {
            if (!byEmployee.TryGetValue(leave.EmployeeId, out var acc))
                continue;

            var days = TimesheetLeaveClassification.OverlapDaysInMonth(
                leave.FromDate,
                leave.ToDate,
                leave.TotalDays,
                periodYm);
            if (days <= 0)
                continue;

            switch (TimesheetLeaveClassification.Classify(
                        leave.LeaveTypeCode,
                        leave.DeductsAnnualBalance))
            {
                case TimesheetLeaveClassification.Kind.Paid:
                    acc.Paid += days;
                    break;
                case TimesheetLeaveClassification.Kind.Unpaid:
                    acc.Unpaid += days;
                    break;
                default:
                    acc.Other += days;
                    break;
            }
        }

        return byEmployee
            .Where(kv => kv.Value.Paid > 0 || kv.Value.Unpaid > 0 || kv.Value.Other > 0)
            .Select(kv => new TimesheetLeaveMergeLine(
                kv.Key,
                kv.Value.Paid,
                kv.Value.Unpaid,
                kv.Value.Other))
            .ToList();
    }

    private sealed class Acc
    {
        public decimal Paid;
        public decimal Unpaid;
        public decimal Other;
    }
}
