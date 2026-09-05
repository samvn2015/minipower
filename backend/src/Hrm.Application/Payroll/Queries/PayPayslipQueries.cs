using Hrm.Application.Common;
using Hrm.Application.Payroll.Dtos;
using Hrm.Domain.Employees;
using Hrm.Domain.Employees.Repositories;
using Hrm.Domain.Identity.Repositories;
using Hrm.Domain.Payroll;
using Hrm.Domain.Payroll.Repositories;
using Hrm.Domain.Shared.Constants;
using Jarvis.Application.Contracts.Queries;
using Jarvis.Domain.Shared.ExceptionHandling;
using Jarvis.Domain.Shared.Messaging;

namespace Hrm.Application.Payroll.Queries;

public sealed record ListMyPayslipsQuery(string? ActorIdpSubject) : IQuery;

public sealed class ListMyPayslipsQueryHandler(
    IIdentityAccountReadRepository accounts,
    IPayPeriodRepository payPeriods)
    : IAsyncQueryHandler<ListMyPayslipsQuery, IReadOnlyList<PayPayslipDto>>
{
    public async Task<IReadOnlyList<PayPayslipDto>> HandleAsync(
        ListMyPayslipsQuery query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);
        IamAccessGuard.RequireAuthenticated(query.ActorIdpSubject);
        var actor = await accounts.FindByIdpSubjectAsync(query.ActorIdpSubject!, cancellationToken)
            .ConfigureAwait(false)
            ?? throw new ForbiddenException(HrmErrorCodes.Forbidden, "Tài khoản không hiệu lực.");

        if (string.IsNullOrWhiteSpace(actor.EmployeeCode))
            return [];

        var rows = await payPeriods
            .ListClosedPayslipsByEmployeeCodeAsync(actor.EmployeeCode, cancellationToken)
            .ConfigureAwait(false);
        return rows.Select(Map).ToList();
    }

    internal static PayPayslipDto Map(PayPayslipSnapshot s) =>
        new(
            s.LineId,
            s.PeriodId,
            s.PeriodYm,
            s.Status.ToString(),
            s.EmployeeId,
            s.EmployeeCode,
            s.WorkDays,
            s.LeaveDaysUnpaid,
            s.LeaveDaysPaid,
            s.NTinh,
            s.TimeWageFactor,
            s.Ot15,
            s.Ot20,
            s.Ot30,
            s.ContractAllowance,
            s.MonthlyAllowance,
            s.BhRate,
            s.TncnRate,
            s.BhAmount,
            s.TncnAmount,
            s.NetPay);
}

public sealed record GetPayslipQuery(string? ActorIdpSubject, Guid PayslipId) : IQuery;

public sealed class GetPayslipQueryHandler(
    IIdentityAccountReadRepository accounts,
    IPayPeriodRepository payPeriods,
    IEmpAuditLogRepository auditLogs)
    : IAsyncQueryHandler<GetPayslipQuery, PayPayslipDto>
{
    public async Task<PayPayslipDto> HandleAsync(
        GetPayslipQuery query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);
        IamAccessGuard.RequireAuthenticated(query.ActorIdpSubject);
        var actor = await accounts.FindByIdpSubjectAsync(query.ActorIdpSubject!, cancellationToken)
            .ConfigureAwait(false)
            ?? throw new ForbiddenException(HrmErrorCodes.Forbidden, "Tài khoản không hiệu lực.");

        var slip = await payPeriods
            .FindPayslipByLineIdAsync(query.PayslipId, cancellationToken)
            .ConfigureAwait(false)
            ?? throw new NotFoundException(HrmErrorCodes.NotFound, "Phiếu lương không tồn tại.");

        if (slip.Status != PayPeriodStatus.Closed)
        {
            throw new NotFoundException(
                HrmErrorCodes.NotFound,
                "Kỳ chưa chốt — chưa có phiếu (PAY-FR-010).");
        }

        PayPayslipAccess.EnsureCanView(actor, slip.EmployeeCode);

        await auditLogs.AppendAsync(
            new EmpAuditLogEntry(
                EmpAuditActions.PayslipViewed,
                slip.EmployeeId,
                slip.LineId,
                query.ActorIdpSubject!,
                slip.PeriodYm),
            cancellationToken).ConfigureAwait(false);

        return ListMyPayslipsQueryHandler.Map(slip);
    }
}
