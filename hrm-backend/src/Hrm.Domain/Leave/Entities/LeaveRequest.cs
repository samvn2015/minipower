using Jarvis.Domain.Entities;

namespace Hrm.Domain.Leave.Entities;

/// <summary>Đơn xin phép — LEV-FR-001 skeleton.</summary>
public class LeaveRequest : BaseEntity<Guid>
{
    public Guid EmployeeId { get; set; }

    public required string LeaveTypeCode { get; set; }

    public LeaveType? LeaveType { get; set; }

    public DateOnly FromDate { get; set; }

    public DateOnly ToDate { get; set; }

    public LeaveDayPart DayPart { get; set; } = LeaveDayPart.FullDay;

    public decimal TotalDays { get; set; }

    public required string Reason { get; set; }

    public Guid HandoverEmployeeId { get; set; }

    public LeaveRequestStatus Status { get; set; } = LeaveRequestStatus.PendingC1;

    public bool IsEmergency { get; set; }

    public DateTime SubmittedAtUtc { get; set; }

    public string? C1ReviewedByIdpSubject { get; set; }

    public DateTime? C1ReviewedAtUtc { get; set; }

    public string? C1ReviewNote { get; set; }

    public string? C2ReviewedByIdpSubject { get; set; }

    public DateTime? C2ReviewedAtUtc { get; set; }

    public string? C2ReviewNote { get; set; }
}
