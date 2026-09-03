using Hrm.Application.Common;
using Hrm.Application.Payroll.Dtos;
using Hrm.Domain.Identity.Repositories;
using Hrm.Domain.Payroll;
using Hrm.Domain.Payroll.Repositories;
using Hrm.Domain.Shared.Constants;
using Jarvis.Application.Contracts.Queries;
using Jarvis.Domain.Shared.ExceptionHandling;
using Jarvis.Domain.Shared.Messaging;

namespace Hrm.Application.Payroll.Queries;

public sealed record GetPayrollPeriodQuery(string? ActorIdpSubject, string PeriodYm) : IQuery;

public sealed class GetPayrollPeriodQueryHandler(
    IIdentityAccountReadRepository accounts,
    IPayPeriodRepository payPeriods,
    IPayRegulationReadRepository regulations,
    IPayWorkdayCalendarRepository calendar)
    : IAsyncQueryHandler<GetPayrollPeriodQuery, PayPeriodDto>
{
    public async Task<PayPeriodDto> HandleAsync(
        GetPayrollPeriodQuery query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);
        IamAccessGuard.RequireAuthenticated(query.ActorIdpSubject);
        var actor = await accounts.FindByIdpSubjectAsync(query.ActorIdpSubject!, cancellationToken)
            .ConfigureAwait(false);
        PayHrGuard.RequireHr(actor);

        var ym = query.PeriodYm.Trim();
        var period = await payPeriods.FindByYmAsync(ym, cancellationToken)
            .ConfigureAwait(false)
            ?? throw new NotFoundException(HrmErrorCodes.NotFound, $"Kỳ PAY {query.PeriodYm} không tồn tại.");

        var standard = await ResolveStandardAsync(ym, regulations, calendar, cancellationToken)
            .ConfigureAwait(false);
        return Map(period, standard);
    }

    internal static async Task<decimal> ResolveStandardAsync(
        string periodYm,
        IPayRegulationReadRepository regulations,
        IPayWorkdayCalendarRepository calendar,
        CancellationToken cancellationToken)
    {
        var fallback = await regulations
            .FindByCodeAsync(PayRegulationCodes.StandardWorkDaysDefault, cancellationToken)
            .ConfigureAwait(false);
        var defaultDays = fallback?.DecimalValue ?? 22m;
        return await calendar
            .ResolveStandardWorkDaysAsync(periodYm, defaultDays, cancellationToken)
            .ConfigureAwait(false);
    }

    internal static PayPeriodDto Map(PayPeriodSnapshot period, decimal standardWorkDays)
    {
        var over = period.Lines
            .Where(l => PayrollWorkdayCap.ExceedsCap(l.NTinh, standardWorkDays))
            .Select(l => l.EmployeeCode)
            .ToList();
        return new PayPeriodDto(
            period.Id,
            period.PeriodYm,
            period.Status.ToString(),
            period.LineCount,
            standardWorkDays,
            over.Count > 0,
            over,
            period.Lines.Select(l => new PayLineDto(
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
}

public sealed record ListPayrollPeriodsQuery(string? ActorIdpSubject) : IQuery;

public sealed class ListPayrollPeriodsQueryHandler(
    IIdentityAccountReadRepository accounts,
    IPayPeriodRepository payPeriods,
    IPayRegulationReadRepository regulations,
    IPayWorkdayCalendarRepository calendar)
    : IAsyncQueryHandler<ListPayrollPeriodsQuery, IReadOnlyList<PayPeriodDto>>
{
    public async Task<IReadOnlyList<PayPeriodDto>> HandleAsync(
        ListPayrollPeriodsQuery query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);
        IamAccessGuard.RequireAuthenticated(query.ActorIdpSubject);
        var actor = await accounts.FindByIdpSubjectAsync(query.ActorIdpSubject!, cancellationToken)
            .ConfigureAwait(false);
        PayHrGuard.RequireHr(actor);

        var rows = await payPeriods.ListAsync(cancellationToken).ConfigureAwait(false);
        var result = new List<PayPeriodDto>(rows.Count);
        foreach (var row in rows)
        {
            var standard = await GetPayrollPeriodQueryHandler
                .ResolveStandardAsync(row.PeriodYm, regulations, calendar, cancellationToken)
                .ConfigureAwait(false);
            result.Add(GetPayrollPeriodQueryHandler.Map(row, standard));
        }

        return result;
    }
}
