using Hrm.Domain.Employees.Constants;
using Hrm.Domain.Employees.Repositories;

namespace Hrm.Domain.Probation;

/// <summary>Fact HĐ thử việc từ EMP — PRB-FR-001 · BR-001 (cấm bịa mốc).</summary>
public static class ProbationContractFacts
{
    public static bool IsActiveProbationContract(EmployeeContractSnapshot? contract)
    {
        if (contract is null)
            return false;

        if (contract.IsProbation)
            return true;

        return string.Equals(
            contract.ContractType,
            EmpContractTypes.Probation,
            StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>BĐ_TV = StartDate HĐ; KT_TV = EndDate HĐ. Không fallback ngày hệ thống.</summary>
    public static (DateOnly? Start, DateOnly? End, bool HasCompleteMilestone) ReadMilestones(
        EmployeeContractSnapshot? contract)
    {
        if (!IsActiveProbationContract(contract))
            return (null, null, false);

        var start = contract!.StartDate;
        var end = contract.EndDate;
        return (start, end, end.HasValue);
    }

    public static DateOnly? ComputeT15Date(DateOnly probationEnd) => probationEnd.AddDays(-15);

    public static DateOnly? ComputeT7Date(DateOnly probationEnd) => probationEnd.AddDays(-7);
}
