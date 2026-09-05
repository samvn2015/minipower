using Jarvis.Domain.Entities;

namespace Hrm.Domain.Payroll.Entities;

/// <summary>Hàng đợi email phiếu lương — PAY-FR-012 (không CC LM).</summary>
public class PayExportOutbox : BaseEntity<Guid>
{
    public required string PeriodYm { get; set; }

    public required string EmployeeCode { get; set; }

    public required string ToAddress { get; set; }

    public string? CcAddress { get; set; }

    public required string Channel { get; set; }

    public required string Subject { get; set; }

    public string? PdfFileName { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    public required string CreatedByIdpSubject { get; set; }
}
