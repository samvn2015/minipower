using Hrm.Domain.Payroll;
using Hrm.Domain.Payroll.Entities;
using Hrm.Domain.Payroll.Repositories;
using Hrm.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Hrm.Infrastructure.Persistence.Repositories;

internal sealed class PayAllowanceRepository(AppDbContext db) : IPayAllowanceRepository
{
    public async Task<IReadOnlyList<PayAllowanceCatalogSnapshot>> ListCatalogAsync(
        CancellationToken cancellationToken = default)
    {
        return await db.PayAllowanceCatalogs.AsNoTracking()
            .OrderBy(x => x.Code)
            .Select(x => new PayAllowanceCatalogSnapshot(x.Code, x.Name, x.IsActive))
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<bool> IsActiveCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await db.PayAllowanceCatalogs.AsNoTracking()
            .AnyAsync(x => x.Code == code && x.IsActive, cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<decimal> SumContractAsync(Guid employeeId, CancellationToken cancellationToken = default)
    {
        var active = IncomeCatalog();
        return await db.PayContractAllowances.AsNoTracking()
            .Where(x => x.EmployeeId == employeeId && x.Code != PayAllowanceCodes.Advance)
            .Join(active, x => x.Code, c => c.Code, (x, _) => x.Amount)
            .SumAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<decimal> SumMonthlyAsync(
        string periodYm,
        Guid employeeId,
        CancellationToken cancellationToken = default)
    {
        var active = IncomeCatalog();
        return await db.PayMonthlyAllowances.AsNoTracking()
            .Where(x => x.PeriodYm == periodYm
                && x.EmployeeId == employeeId
                && x.Code != PayAllowanceCodes.Advance)
            .Join(active, x => x.Code, c => c.Code, (x, _) => x.Amount)
            .SumAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<decimal> SumMealTaxExemptAsync(
        string periodYm,
        Guid employeeId,
        CancellationToken cancellationToken = default)
    {
        var active = IncomeCatalog();
        var contract = await db.PayContractAllowances.AsNoTracking()
            .Where(x => x.EmployeeId == employeeId && x.Code == PayAllowanceCodes.Meal)
            .Join(active, x => x.Code, c => c.Code, (x, _) => x.Amount)
            .SumAsync(cancellationToken)
            .ConfigureAwait(false);
        var monthly = await db.PayMonthlyAllowances.AsNoTracking()
            .Where(x => x.PeriodYm == periodYm
                && x.EmployeeId == employeeId
                && x.Code == PayAllowanceCodes.Meal)
            .Join(active, x => x.Code, c => c.Code, (x, _) => x.Amount)
            .SumAsync(cancellationToken)
            .ConfigureAwait(false);
        return contract + monthly;
    }

    public async Task<decimal> SumAdvanceAsync(
        string periodYm,
        Guid employeeId,
        CancellationToken cancellationToken = default)
    {
        var active = db.PayAllowanceCatalogs.AsNoTracking().Where(c => c.IsActive);
        return await db.PayMonthlyAllowances.AsNoTracking()
            .Where(x => x.PeriodYm == periodYm
                && x.EmployeeId == employeeId
                && x.Code == PayAllowanceCodes.Advance)
            .Join(active, x => x.Code, c => c.Code, (x, _) => x.Amount)
            .SumAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<string>> ListUnknownMonthlyCodesAsync(
        string periodYm,
        CancellationToken cancellationToken = default)
    {
        var active = db.PayAllowanceCatalogs.AsNoTracking().Where(c => c.IsActive).Select(c => c.Code);
        return await db.PayMonthlyAllowances.AsNoTracking()
            .Where(x => x.PeriodYm == periodYm && !active.Contains(x.Code))
            .Select(x => x.Code)
            .Distinct()
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<PayMonthlyAllowanceSnapshot>> ListMonthlyByYmAsync(
        string periodYm,
        CancellationToken cancellationToken = default)
    {
        return await db.PayMonthlyAllowances.AsNoTracking()
            .Where(x => x.PeriodYm == periodYm)
            .OrderBy(x => x.EmployeeCode)
            .ThenBy(x => x.Code)
            .Select(x => new PayMonthlyAllowanceSnapshot(
                x.Id, x.PeriodYm, x.EmployeeId, x.EmployeeCode, x.Code, x.Amount))
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task UpsertMonthlyAsync(
        string periodYm,
        Guid employeeId,
        string employeeCode,
        string code,
        decimal amount,
        CancellationToken cancellationToken = default)
    {
        var entity = await db.PayMonthlyAllowances
            .FirstOrDefaultAsync(
                x => x.PeriodYm == periodYm && x.EmployeeId == employeeId && x.Code == code,
                cancellationToken)
            .ConfigureAwait(false);

        if (entity is null)
        {
            db.PayMonthlyAllowances.Add(new PayMonthlyAllowance
            {
                Id = Guid.NewGuid(),
                PeriodYm = periodYm,
                EmployeeId = employeeId,
                EmployeeCode = employeeCode,
                Code = code,
                Amount = amount
            });
        }
        else
        {
            entity.Amount = amount;
            entity.EmployeeCode = employeeCode;
        }

        await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    private IQueryable<PayAllowanceCatalog> IncomeCatalog() =>
        db.PayAllowanceCatalogs.AsNoTracking()
            .Where(c => c.IsActive && c.Code != PayAllowanceCodes.Advance);
}
