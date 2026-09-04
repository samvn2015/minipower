namespace Hrm.Domain.Payroll.Repositories;

public sealed record PayExportOutboxCreateModel(
    string PeriodYm,
    string EmployeeCode,
    string ToAddress,
    string? CcAddress,
    string Channel,
    string Subject,
    string? PdfFileName,
    string CreatedByIdpSubject);

public interface IPayExportOutboxRepository
{
    Task AddManyAsync(
        IReadOnlyList<PayExportOutboxCreateModel> rows,
        CancellationToken cancellationToken = default);
}
