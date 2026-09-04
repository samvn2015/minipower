using Jarvis.Domain.Entities;

namespace Hrm.Domain.Leave.Entities;

/// <summary>Outbox thông báo phép — Email/InApp only (LEV-FR-009).</summary>
public class LeaveNotification : BaseEntity<Guid>
{
    public Guid LeaveRequestId { get; set; }

    public Guid EmployeeId { get; set; }

    public required string EventType { get; set; }

    public required string Channel { get; set; }

    public required string Message { get; set; }

    public DateTime CreatedAtUtc { get; set; }
}
