namespace Hrm.Domain.Employees.Entities;

/// <summary>Catalog trình độ học vấn — EMP-FR-017.</summary>
public class EducationLevel
{
    public required string Code { get; set; }

    public required string Name { get; set; }

    public EducationLevelStatus Status { get; set; } = EducationLevelStatus.Active;
}
