using Hrm.Domain.Payroll;
using Hrm.Domain.Payroll.Entities;
using Hrm.Domain.Payroll.Repositories;
using Hrm.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Hrm.Infrastructure.Persistence.Repositories;

internal sealed class PayPeriodRepository(AppDbContext db) : IPayPeriodRepository
{
    public async Task<bool> IsClosedAsync(string periodYm, CancellationToken cancellationToken = default)
    {
        var status = await db.PayPeriods.AsNoTracking()
            .Where(x => x.PeriodYm == periodYm)
            .Select(x => (PayPeriodStatus?)x.Status)
            .FirstOrDefaultAsync(cancellationToken)
            .ConfigureAwait(false);
        return status == PayPeriodStatus.Closed;
    }

    public async Task MarkClosedAsync(
        string periodYm,
        string closedByIdpSubject,
        CancellationToken cancellationToken = default)
    {
        var entity = await db.PayPeriods
            .FirstOrDefaultAsync(x => x.PeriodYm == periodYm, cancellationToken)
            .ConfigureAwait(false);

        if (entity is null)
        {
            entity = new PayPeriod
            {
                Id = Guid.NewGuid(),
                PeriodYm = periodYm,
                Status = PayPeriodStatus.Closed,
                ClosedAtUtc = DateTime.UtcNow,
                ClosedByIdpSubject = closedByIdpSubject
            };
            db.PayPeriods.Add(entity);
        }
        else
        {
            entity.Status = PayPeriodStatus.Closed;
            entity.ClosedAtUtc = DateTime.UtcNow;
            entity.ClosedByIdpSubject = closedByIdpSubject;
        }

        await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}
