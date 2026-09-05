using Hrm.Domain.Payroll.Entities;
using Hrm.Domain.Payroll.Repositories;
using Hrm.Infrastructure.Persistence;

namespace Hrm.Infrastructure.Persistence.Repositories;

internal sealed class PayExportOutboxRepository(AppDbContext db) : IPayExportOutboxRepository
{
    public async Task AddManyAsync(
        IReadOnlyList<PayExportOutboxCreateModel> rows,
        CancellationToken cancellationToken = default)
    {
        foreach (var row in rows)
        {
            db.PayExportOutboxes.Add(new PayExportOutbox
            {
                Id = Guid.NewGuid(),
                PeriodYm = row.PeriodYm,
                EmployeeCode = row.EmployeeCode,
                ToAddress = row.ToAddress,
                CcAddress = row.CcAddress,
                Channel = row.Channel,
                Subject = row.Subject,
                PdfFileName = row.PdfFileName,
                CreatedAtUtc = DateTime.UtcNow,
                CreatedByIdpSubject = row.CreatedByIdpSubject
            });
        }

        await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}
