using Jarvis.Domain.Entities;

namespace Hrm.Domain.Timekeeping.Entities;

/// <summary>Cột mẫu từ master — TIM-FR-002 / TIM-BR-002 (không hardcode URD).</summary>
public class TimesheetTemplateColumn : BaseEntity<Guid>
{
    public Guid TemplateVersionId { get; set; }

    public TimesheetTemplateVersion? TemplateVersion { get; set; }

    public required string ColumnKey { get; set; }

    public required string DisplayName { get; set; }

    public int SortOrder { get; set; }

    public bool IsRequired { get; set; }

    /// <summary>Mapping nghiệp vụ: EmployeeCode, WorkDays, Ot15, Ot20, Ot30, …</summary>
    public required string MapsTo { get; set; }
}
