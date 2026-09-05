namespace Hrm.Domain.Employees.Entities;

/// <summary>Master công thức thâm niên — EMP-FR-010 (không hardcode trên code).</summary>
public class SeniorityRule
{
    public required string Code { get; set; }

    public SeniorityBasisType BasisType { get; set; } = SeniorityBasisType.ContractStartDate;

    public SeniorityRuleStatus Status { get; set; } = SeniorityRuleStatus.Active;
}
