namespace Hrm.Domain.Payroll.Repositories;

public sealed record PayAllowanceCatalogSnapshot(string Code, string Name, bool IsActive);

public sealed record PayMonthlyAllowanceSnapshot(
    Guid Id,
    string PeriodYm,
    Guid EmployeeId,
    string EmployeeCode,
    string Code,
    decimal Amount);

public interface IPayAllowanceRepository
{
    Task<IReadOnlyList<PayAllowanceCatalogSnapshot>> ListCatalogAsync(
        CancellationToken cancellationToken = default);

    Task<bool> IsActiveCodeAsync(string code, CancellationToken cancellationToken = default);

    /// <summary>Tổng PC thu nhập HĐ (không gồm tạm ứng).</summary>
    Task<decimal> SumContractAsync(Guid employeeId, CancellationToken cancellationToken = default);

    /// <summary>Tổng PC thu nhập tháng (không gồm tạm ứng).</summary>
    Task<decimal> SumMonthlyAsync(
        string periodYm,
        Guid employeeId,
        CancellationToken cancellationToken = default);

    /// <summary>PC ăn trưa (HĐ + tháng) — miễn trừ TNCN theo C&amp;B.</summary>
    Task<decimal> SumMealTaxExemptAsync(
        string periodYm,
        Guid employeeId,
        CancellationToken cancellationToken = default);

    /// <summary>Tạm ứng tháng — trừ thực lĩnh, không cộng gross.</summary>
    Task<decimal> SumAdvanceAsync(
        string periodYm,
        Guid employeeId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<string>> ListUnknownMonthlyCodesAsync(
        string periodYm,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<PayMonthlyAllowanceSnapshot>> ListMonthlyByYmAsync(
        string periodYm,
        CancellationToken cancellationToken = default);

    Task UpsertMonthlyAsync(
        string periodYm,
        Guid employeeId,
        string employeeCode,
        string code,
        decimal amount,
        CancellationToken cancellationToken = default);
}
