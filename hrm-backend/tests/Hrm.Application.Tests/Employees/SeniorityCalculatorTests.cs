using Hrm.Application.Employees;
using Hrm.Domain.Employees;
using Hrm.Domain.Employees.Repositories;

namespace Hrm.Application.Tests.Employees;

public sealed class SeniorityCalculatorTests
{
    [Fact]
    public void Calculate_UsesContractStartFromMasterRule()
    {
        var snapshot = EmpTestSnapshots.DevEmployee(
            Guid.NewGuid(),
            contract: new EmployeeContractSnapshot("PROBATION", new DateOnly(2020, 1, 15), null, true));

        var result = SeniorityCalculator.Calculate(
            snapshot,
            new SeniorityRuleSnapshot("SR-DEFAULT", SeniorityBasisType.ContractStartDate),
            new DateOnly(2026, 8, 29));

        Assert.NotNull(result);
        Assert.Equal("SR-DEFAULT", result!.RuleCode);
        Assert.True(result.Years >= 6);
    }

    [Fact]
    public void Calculate_ReturnsDashWhenNoStartDate()
    {
        var snapshot = EmpTestSnapshots.DevEmployee(Guid.NewGuid());

        var result = SeniorityCalculator.Calculate(
            snapshot,
            new SeniorityRuleSnapshot("SR-DEFAULT", SeniorityBasisType.ContractStartDate),
            new DateOnly(2026, 8, 29));

        Assert.NotNull(result);
        Assert.Equal("—", result!.DisplayText);
    }
}
