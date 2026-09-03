using Jarvis.Domain.Entities;

namespace Hrm.Domain.Timekeeping.Entities;

/// <summary>Batch import preview — TIM-FR-003/004.</summary>
public class TimesheetImportBatch : BaseEntity<Guid>
{
    public required string PeriodYm { get; set; }

    public Guid TemplateVersionId { get; set; }

    public required string TemplateVersionCode { get; set; }

    public TimesheetImportBatchStatus Status { get; set; } = TimesheetImportBatchStatus.Preview;

    public required string UploadedByIdpSubject { get; set; }

    public DateTime UploadedAtUtc { get; set; }

    public string? FileName { get; set; }

    public int TotalRows { get; set; }

    public int ErrorRows { get; set; }

    public bool HasMustErrors { get; set; }

    public ICollection<TimesheetImportRow> Rows { get; set; } = [];
}

/// <summary>Dòng preview import.</summary>
public class TimesheetImportRow : BaseEntity<Guid>
{
    public Guid BatchId { get; set; }

    public TimesheetImportBatch? Batch { get; set; }

    public int RowNumber { get; set; }

    public string? EmployeeCode { get; set; }

    public Guid? EmployeeId { get; set; }

    public decimal? WorkDays { get; set; }

    public decimal? Ot15 { get; set; }

    public decimal? Ot20 { get; set; }

    public decimal? Ot30 { get; set; }

    public bool IsOk { get; set; }

    public string? ErrorCode { get; set; }

    public string? ErrorMessage { get; set; }
}

/// <summary>Kỳ công tháng — Draft sau commit (TIM-FR-005).</summary>
public class TimesheetPeriod : BaseEntity<Guid>
{
    public required string PeriodYm { get; set; }

    public TimesheetPeriodStatus Status { get; set; } = TimesheetPeriodStatus.Draft;

    public Guid? SourceImportBatchId { get; set; }

    public DateTime? CommittedAtUtc { get; set; }

    public string? CommittedByIdpSubject { get; set; }

    public ICollection<TimesheetLine> Lines { get; set; } = [];
}

/// <summary>Dòng bảng công Draft.</summary>
public class TimesheetLine : BaseEntity<Guid>
{
    public Guid PeriodId { get; set; }

    public TimesheetPeriod? Period { get; set; }

    public Guid EmployeeId { get; set; }

    public required string EmployeeCode { get; set; }

    public decimal WorkDays { get; set; }

    public decimal Ot15 { get; set; }

    public decimal Ot20 { get; set; }

    public decimal Ot30 { get; set; }
}
