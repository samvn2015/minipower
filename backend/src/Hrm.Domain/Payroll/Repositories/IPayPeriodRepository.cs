namespace Hrm.Domain.Payroll.Repositories;

public sealed record PayLineSnapshot(
    Guid Id,
    Guid EmployeeId,
    string EmployeeCode,
    decimal WorkDays,
    decimal LeaveDaysUnpaid,
    decimal LeaveDaysPaid,
    decimal NTinh,
    decimal TimeWageFactor,
    decimal Ot15,
    decimal Ot20,
    decimal Ot30,
    decimal ContractAllowance,
    decimal MonthlyAllowance,
    decimal BhRate,
    decimal TncnRate,
    decimal BhAmount,
    decimal TncnAmount,
    decimal NetPay);

public sealed record PayPeriodSnapshot(
    Guid Id,
    string PeriodYm,
    PayPeriodStatus Status,
    int LineCount,
    IReadOnlyList<PayLineSnapshot> Lines);

public sealed record PayLineCreateModel(
    Guid EmployeeId,
    string EmployeeCode,
    decimal WorkDays,
    decimal LeaveDaysUnpaid,
    decimal LeaveDaysPaid,
    decimal NTinh,
    decimal TimeWageFactor,
    decimal Ot15,
    decimal Ot20,
    decimal Ot30,
    decimal ContractAllowance,
    decimal MonthlyAllowance,
    decimal BhRate,
    decimal TncnRate,
    decimal BhAmount,
    decimal TncnAmount,
    decimal NetPay);

public interface IPayPeriodGate
{
    /// <summary>True nếu kỳ PAY đã chốt — cấm bỏ chốt TIM (TIM-FR-012).</summary>
    Task<bool> IsClosedAsync(string periodYm, CancellationToken cancellationToken = default);
}

public interface IPayPeriodRepository : IPayPeriodGate
{
    Task<PayPeriodSnapshot?> FindByYmAsync(string periodYm, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<PayPeriodSnapshot>> ListAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Tạo/ghi đè Draft từ dòng TIM. Returns null nếu kỳ PAY đã Closed.
    /// </summary>
    Task<PayPeriodSnapshot?> RunDraftAsync(
        string periodYm,
        string ranByIdpSubject,
        IReadOnlyList<PayLineCreateModel> lines,
        CancellationToken cancellationToken = default);

    /// <summary>Đánh dấu kỳ PAY Closed (TIM unlock gate + PAY-SCR-003 stub).</summary>
    Task MarkClosedAsync(string periodYm, string closedByIdpSubject, CancellationToken cancellationToken = default);

    Task<PayPayslipSnapshot?> FindPayslipByLineIdAsync(
        Guid lineId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<PayPayslipSnapshot>> ListClosedPayslipsByEmployeeCodeAsync(
        string employeeCode,
        CancellationToken cancellationToken = default);
}

public sealed record PayPayslipSnapshot(
    Guid LineId,
    Guid PeriodId,
    string PeriodYm,
    PayPeriodStatus Status,
    Guid EmployeeId,
    string EmployeeCode,
    decimal WorkDays,
    decimal LeaveDaysUnpaid,
    decimal LeaveDaysPaid,
    decimal NTinh,
    decimal TimeWageFactor,
    decimal Ot15,
    decimal Ot20,
    decimal Ot30,
    decimal ContractAllowance,
    decimal MonthlyAllowance,
    decimal BhRate,
    decimal TncnRate,
    decimal BhAmount,
    decimal TncnAmount,
    decimal NetPay);
