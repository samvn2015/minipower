using Jarvis.Domain.Entities;

namespace Hrm.Domain.Lifecycle.Entities;

/// <summary>Outbox khóa Git + CRM SP cùng hàng — cấm CRM sales (LIF-FR-010).</summary>
public class LifAccessLockOutbox : BaseEntity<Guid>
{
    public Guid CaseId { get; set; }

    public Guid EmployeeId { get; set; }

    public required string EmployeeCode { get; set; }

    /// <summary>Luôn <c>Git;CrmSp</c> — không sales.</summary>
    public required string TargetSystems { get; set; }

    /// <summary>Channel connector: <c>git+crm-sp</c>.</summary>
    public required string Channel { get; set; }

    public DateOnly AsOfDate { get; set; }

    public bool IsEarlySecurityCr { get; set; }

    public string? CrReason { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    public required string CreatedByIdpSubject { get; set; }
}
