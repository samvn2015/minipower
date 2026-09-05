namespace Hrm.Domain.Employees.Entities;

/// <summary>HĐ hiện tại — EMP-FR-005 · DOC-11 Contract.</summary>
public class EmployeeContract
{
    public Guid Id { get; set; }

    public Guid EmployeeId { get; set; }

    public Employee Employee { get; set; } = null!;

    /// <summary>Mã loại HĐ từ master (<see cref="Constants.EmpContractTypes"/>).</summary>
    public required string ContractType { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    /// <summary>Kết thúc thử việc (KT_TV) — fact PRB/PAY.</summary>
    public bool IsProbation { get; set; }
}
