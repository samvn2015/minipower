namespace Hrm.Domain.Payroll.Repositories;

public sealed record PayWorkdayCalendarSnapshot(string PeriodYm, decimal StandardWorkDays);

public interface IPayWorkdayCalendarRepository
{
    Task<decimal> ResolveStandardWorkDaysAsync(
        string periodYm,
        decimal defaultStandardWorkDays,
        CancellationToken cancellationToken = default);

    Task UpsertAsync(
        string periodYm,
        decimal standardWorkDays,
        CancellationToken cancellationToken = default);
}
