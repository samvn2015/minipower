using Hrm.Domain.Employees.Constants;
using Hrm.Domain.Employees.Repositories;

namespace Hrm.Domain.Payroll;

/// <summary>Hệ số lương thời gian theo HĐ tại kỳ — PAY-FR-003 · PAY-BR-003.</summary>
public static class PayrollTimeWageFactor
{
    public const decimal OfficialFactor = 1.00m;

    public static bool IsProbationInPeriod(EmployeeContractSnapshot? contract, string periodYm)
    {
        if (contract is null)
            return false;

        var isProbationType = contract.IsProbation
            || string.Equals(contract.ContractType, EmpContractTypes.Probation, StringComparison.OrdinalIgnoreCase);
        if (!isProbationType)
            return false;

        if (!TryParseYm(periodYm, out var monthStart, out var monthEnd))
            return false;

        if (contract.StartDate > monthEnd)
            return false;

        if (contract.EndDate is { } end && end < monthStart)
            return false;

        return true;
    }

    public static decimal Resolve(
        EmployeeContractSnapshot? contract,
        string periodYm,
        decimal probationFactorFromMaster)
    {
        return IsProbationInPeriod(contract, periodYm)
            ? probationFactorFromMaster
            : OfficialFactor;
    }

    private static bool TryParseYm(string periodYm, out DateOnly monthStart, out DateOnly monthEnd)
    {
        monthStart = default;
        monthEnd = default;
        if (periodYm.Length != 7
            || periodYm[4] != '-'
            || !int.TryParse(periodYm.AsSpan(0, 4), out var year)
            || !int.TryParse(periodYm.AsSpan(5, 2), out var month)
            || month is < 1 or > 12)
        {
            return false;
        }

        monthStart = new DateOnly(year, month, 1);
        monthEnd = monthStart.AddMonths(1).AddDays(-1);
        return true;
    }
}
