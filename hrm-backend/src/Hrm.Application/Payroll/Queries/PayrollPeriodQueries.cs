using Hrm.Application.Common;
using Hrm.Application.Payroll.Dtos;
using Hrm.Domain.Identity.Repositories;
using Hrm.Domain.Payroll.Repositories;
using Hrm.Domain.Shared.Constants;
using Jarvis.Application.Contracts.Queries;
using Jarvis.Domain.Shared.ExceptionHandling;
using Jarvis.Domain.Shared.Messaging;

namespace Hrm.Application.Payroll.Queries;

public sealed record GetPayrollPeriodQuery(string? ActorIdpSubject, string PeriodYm) : IQuery;

public sealed class GetPayrollPeriodQueryHandler(
    IIdentityAccountReadRepository accounts,
    IPayPeriodRepository payPeriods)
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

        var period = await payPeriods.FindByYmAsync(query.PeriodYm.Trim(), cancellationToken)
            .ConfigureAwait(false)
            ?? throw new NotFoundException(HrmErrorCodes.NotFound, $"Kỳ PAY {query.PeriodYm} không tồn tại.");

        return Map(period);
    }

    internal static PayPeriodDto Map(PayPeriodSnapshot period) =>
        new(
            period.Id,
            period.PeriodYm,
            period.Status.ToString(),
            period.LineCount,
            period.Lines.Select(l => new PayLineDto(
                l.Id,
                l.EmployeeId,
                l.EmployeeCode,
                l.WorkDays,
                l.LeaveDaysUnpaid,
                l.LeaveDaysPaid,
                l.NTinh,
                l.Ot15,
                l.Ot20,
                l.Ot30)).ToList());
}

public sealed record ListPayrollPeriodsQuery(string? ActorIdpSubject) : IQuery;

public sealed class ListPayrollPeriodsQueryHandler(
    IIdentityAccountReadRepository accounts,
    IPayPeriodRepository payPeriods)
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
        return rows.Select(GetPayrollPeriodQueryHandler.Map).ToList();
    }
}
