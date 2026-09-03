using Jarvis.Domain.Entities;

namespace Hrm.Domain.Timekeeping.Entities;

/// <summary>Version mẫu Excel công — TIM-FR-001 / TIM-BR-001.</summary>
public class TimesheetTemplateVersion : BaseEntity<Guid>
{
    public required string VersionCode { get; set; }

    public required string Name { get; set; }

    public TimesheetTemplateStatus Status { get; set; } = TimesheetTemplateStatus.Draft;

    public DateTime? PublishedAtUtc { get; set; }

    public string? PublishedByIdpSubject { get; set; }

    public ICollection<TimesheetTemplateColumn> Columns { get; set; } = [];
}
