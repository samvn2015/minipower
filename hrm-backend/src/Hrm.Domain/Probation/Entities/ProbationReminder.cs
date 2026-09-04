using Jarvis.Domain.Entities;

namespace Hrm.Domain.Probation.Entities;

/// <summary>Cảnh báo T-15 / task T-7 — PRB-FR-002 · FR-003 · FR-011 (in-app + email; không CRM sales).</summary>
public class ProbationReminder : BaseEntity<Guid>
{
    public ProbationReminderKind Kind { get; set; }

    public Guid EmployeeId { get; set; }

    public required string EmployeeCode { get; set; }

    public DateOnly ProbationEndDate { get; set; }

    public DateOnly DueDate { get; set; }

    public DateOnly AsOfDate { get; set; }

    /// <summary>LM được gán T-7; null = gán HR (FR-014).</summary>
    public Guid? AssigneeEmployeeId { get; set; }

    public string? AssigneeEmployeeCode { get; set; }

    public required string InAppMessage { get; set; }

    public required string EmailTo { get; set; }

    public required string Channel { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    public required string CreatedByIdpSubject { get; set; }
}
