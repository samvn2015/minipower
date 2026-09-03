using Hrm.Domain.Employees.Repositories;
using Hrm.Domain.Payroll;

namespace Hrm.Application.Tests.Payroll;

public sealed class PayrollTimeWageFactorTests
{
    [Fact]
    public void Resolve_ProbationInMonth_UsesMasterFactor()
    {
        var contract = new EmployeeContractSnapshot(
            "PROBATION",
            new DateOnly(2027, 1, 1),
            null,
            IsProbation: true);

        Assert.Equal(0.85m, PayrollTimeWageFactor.Resolve(contract, "2027-07", 0.85m));
    }

    [Fact]
    public void Resolve_Official_IsOne()
    {
        var contract = new EmployeeContractSnapshot(
            "OFFICIAL",
            new DateOnly(2026, 1, 1),
            null,
            IsProbation: false);

        Assert.Equal(1.00m, PayrollTimeWageFactor.Resolve(contract, "2027-07", 0.85m));
    }

    [Fact]
    public void Resolve_ProbationEndedBeforeMonth_IsOne()
    {
        var contract = new EmployeeContractSnapshot(
            "PROBATION",
            new DateOnly(2026, 1, 1),
            new DateOnly(2026, 6, 30),
            IsProbation: true);

        Assert.Equal(1.00m, PayrollTimeWageFactor.Resolve(contract, "2027-07", 0.85m));
    }
}
