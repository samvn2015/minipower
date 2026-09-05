using Hrm.Domain.Employees;

namespace Hrm.Domain.Employees.Entities;

/// <summary>Catalog org — EMP-FR-004.</summary>
public class OrgUnit
{
    public required string Code { get; set; }

    public required string Name { get; set; }

    public OrgUnitStatus Status { get; set; } = OrgUnitStatus.Active;
}
