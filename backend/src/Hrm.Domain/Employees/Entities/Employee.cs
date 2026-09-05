using Jarvis.Domain.Entities;

namespace Hrm.Domain.Employees.Entities;

/// <summary>DOC-11 Employee — khung EMP skeleton.</summary>
public class Employee : BaseEntity<Guid>
{
    /// <summary>Mã nhân viên (MNV) — unique.</summary>
    public required string EmployeeCode { get; set; }

    public string? FullName { get; set; }

    public string? Cccd { get; set; }

    public string? EmailCty { get; set; }

    /// <summary>Mã số thuế (MST).</summary>
    public string? TaxId { get; set; }

    /// <summary>Mã đơn vị org (catalog).</summary>
    public string? OrgUnitCode { get; set; }

    public OrgUnit? OrgUnit { get; set; }

    /// <summary>Mã bậc học vấn (catalog) — EMP-FR-017.</summary>
    public string? EducationLevelCode { get; set; }

    public EducationLevel? EducationLevel { get; set; }

    /// <summary>Mốc tính thâm niên tùy chọn; mặc định lấy từ HĐ theo master.</summary>
    public DateOnly? SeniorityStartDate { get; set; }

    public Guid? LineManagerEmployeeId { get; set; }

    public EmployeeContract? Contract { get; set; }

    public ICollection<LineManagerChangeRequest> LineManagerChangeRequests { get; set; } =
        new List<LineManagerChangeRequest>();

    public EmployeeStatus Status { get; set; } = EmployeeStatus.Active;
}
