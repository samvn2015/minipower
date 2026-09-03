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

    public async Task<PayPeriodSnapshot?> FindByYmAsync(
        string periodYm,
        CancellationToken cancellationToken = default)
    {
        var entity = await db.PayPeriods.AsNoTracking()
            .Include(x => x.Lines)
            .FirstOrDefaultAsync(x => x.PeriodYm == periodYm, cancellationToken)
            .ConfigureAwait(false);
        return entity is null ? null : Map(entity);
    }

    public async Task<IReadOnlyList<PayPeriodSnapshot>> ListAsync(
        CancellationToken cancellationToken = default)
    {
        var rows = await db.PayPeriods.AsNoTracking()
            .Include(x => x.Lines)
            .OrderByDescending(x => x.PeriodYm)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
        return rows.Select(Map).ToList();
    }

    public async Task<PayPeriodSnapshot?> RunDraftAsync(
        string periodYm,
        string ranByIdpSubject,
        IReadOnlyList<PayLineCreateModel> lines,
        CancellationToken cancellationToken = default)
    {
        var entity = await db.PayPeriods
            .Include(x => x.Lines)
            .FirstOrDefaultAsync(x => x.PeriodYm == periodYm, cancellationToken)
            .ConfigureAwait(false);

        if (entity is { Status: PayPeriodStatus.Closed })
            return null;

        if (entity is null)
        {
            entity = new PayPeriod
            {
                Id = Guid.NewGuid(),
                PeriodYm = periodYm,
                Status = PayPeriodStatus.Draft
            };
            db.PayPeriods.Add(entity);
        }
        else
        {
            db.PayLines.RemoveRange(entity.Lines);
            entity.Lines.Clear();
        }

        foreach (var line in lines)
        {
            entity.Lines.Add(new PayLine
            {
                Id = Guid.NewGuid(),
                PeriodId = entity.Id,
                EmployeeId = line.EmployeeId,
                EmployeeCode = line.EmployeeCode,
                WorkDays = line.WorkDays,
                LeaveDaysUnpaid = line.LeaveDaysUnpaid,
                LeaveDaysPaid = line.LeaveDaysPaid,
                NTinh = line.NTinh,
                TimeWageFactor = line.TimeWageFactor,
                Ot15 = line.Ot15,
                Ot20 = line.Ot20,
                Ot30 = line.Ot30
            });
        }

        entity.Status = PayPeriodStatus.Draft;
        entity.RanAtUtc = DateTime.UtcNow;
        entity.RanByIdpSubject = ranByIdpSubject;
        entity.ClosedAtUtc = null;
        entity.ClosedByIdpSubject = null;

        await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Map(entity);
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

    private static PayPeriodSnapshot Map(PayPeriod entity) =>
        new(
            entity.Id,
            entity.PeriodYm,
            entity.Status,
            entity.Lines.Count,
            entity.Lines.Select(l => new PayLineSnapshot(
                l.Id,
                l.EmployeeId,
                l.EmployeeCode,
                l.WorkDays,
                l.LeaveDaysUnpaid,
                l.LeaveDaysPaid,
                l.NTinh,
                l.TimeWageFactor,
                l.Ot15,
                l.Ot20,
                l.Ot30)).ToList());
}
