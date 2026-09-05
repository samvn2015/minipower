using Hrm.Domain.Employees;
using Hrm.Domain.Employees.Repositories;

namespace Hrm.Application.Employees;

public static class SeniorityCalculator
{
    public static Dtos.SeniorityDto? Calculate(
        EmployeeSnapshot employee,
        SeniorityRuleSnapshot? rule,
        DateOnly today)
    {
        if (rule is null)
            return null;

        var start = ResolveStartDate(employee, rule.BasisType);
        if (start is null)
            return new Dtos.SeniorityDto(0, 0, "—", rule.Code);

        var (years, months) = DiffYearMonth(start.Value, today);
        return new Dtos.SeniorityDto(years, months, $"{years} năm {months} tháng", rule.Code);
    }

    internal static DateOnly? ResolveStartDate(EmployeeSnapshot employee, SeniorityBasisType basisType) =>
        basisType switch
        {
            SeniorityBasisType.SeniorityStartDate => employee.SeniorityStartDate ?? employee.Contract?.StartDate,
            _ => employee.SeniorityStartDate ?? employee.Contract?.StartDate,
        };

    private static (int Years, int Months) DiffYearMonth(DateOnly start, DateOnly end)
    {
        if (end < start)
            return (0, 0);

        var months = (end.Year - start.Year) * 12 + (end.Month - start.Month);
        if (end.Day < start.Day)
            months--;

        if (months < 0)
            return (0, 0);

        return (months / 12, months % 12);
    }
}
