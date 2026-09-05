using Hrm.Domain.Payroll.Entities;
using Hrm.Domain.Payroll.Repositories;
using Hrm.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Hrm.Infrastructure.Persistence.Repositories;

internal sealed class PayWorkdayCalendarRepository(AppDbContext db) : IPayWorkdayCalendarRepository
{
    public async Task<decimal> ResolveStandardWorkDaysAsync(
        string periodYm,
        decimal defaultStandardWorkDays,
        CancellationToken cancellationToken = default)
    {
        var overrideDays = await db.PayWorkdayCalendars.AsNoTracking()
            .Where(x => x.PeriodYm == periodYm)
            .Select(x => (decimal?)x.StandardWorkDays)
            .FirstOrDefaultAsync(cancellationToken)
            .ConfigureAwait(false);
        return overrideDays ?? defaultStandardWorkDays;
    }

    public async Task UpsertAsync(
        string periodYm,
        decimal standardWorkDays,
        CancellationToken cancellationToken = default)
    {
        var entity = await db.PayWorkdayCalendars
            .FirstOrDefaultAsync(x => x.PeriodYm == periodYm, cancellationToken)
            .ConfigureAwait(false);

        if (entity is null)
        {
            db.PayWorkdayCalendars.Add(new PayWorkdayCalendar
            {
                Id = Guid.NewGuid(),
                PeriodYm = periodYm,
                StandardWorkDays = standardWorkDays
            });
        }
        else
        {
            entity.StandardWorkDays = standardWorkDays;
        }

        await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}
