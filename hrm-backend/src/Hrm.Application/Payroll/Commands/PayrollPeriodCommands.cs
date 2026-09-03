using Hrm.Application.Common;
using Hrm.Application.Payroll.Dtos;
using Hrm.Domain.Employees.Repositories;
using Hrm.Domain.Identity.Repositories;
using Hrm.Domain.Payroll;
using Hrm.Domain.Payroll.Repositories;
using Hrm.Domain.Shared.Constants;
using Hrm.Domain.Timekeeping;
using Hrm.Domain.Timekeeping.Repositories;
using Jarvis.Application.Contracts.Commands;
using Jarvis.Domain.Shared.ExceptionHandling;
using Jarvis.Domain.Shared.Messaging;

namespace Hrm.Application.Payroll.Commands;

public sealed record RunPayrollPeriodCommand(string? ActorIdpSubject, string PeriodYm) : ICommand;

public sealed class RunPayrollPeriodCommandHandler(
    IIdentityAccountReadRepository accounts,
    ITimesheetImportRepository timesheets,
    IPayPeriodRepository payPeriods,
    IEmployeeReadRepository employees,
    IPayRegulationReadRepository regulations)
    : IAsyncCommandHandler<RunPayrollPeriodCommand, PayRunResult>
{
    public async Task<PayRunResult> HandleAsync(
        RunPayrollPeriodCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);
        IamAccessGuard.RequireAuthenticated(command.ActorIdpSubject);
        var actor = await accounts.FindByIdpSubjectAsync(command.ActorIdpSubject!, cancellationToken)
            .ConfigureAwait(false);
        PayHrGuard.RequireHr(actor);

        if (string.IsNullOrWhiteSpace(command.PeriodYm)
            || command.PeriodYm.Length != 7
            || command.PeriodYm[4] != '-')
        {
            throw new BadRequestException(HrmErrorCodes.BadRequest, "PeriodYm phải dạng YYYY-MM.");
        }

        var ym = command.PeriodYm.Trim();
        var tim = await timesheets.FindPeriodByYmAsync(ym, cancellationToken).ConfigureAwait(false)
            ?? throw new BadRequestException(
                HrmErrorCodes.BadRequest,
                $"Chưa có kỳ công TIM {ym} — cấm tính lương (PAY-FR-001).");

        if (tim.Status != TimesheetPeriodStatus.Closed)
        {
            throw new BadRequestException(
                HrmErrorCodes.BadRequest,
                $"TIM {ym} chưa chốt (hiện: {tim.Status}) — cấm tính lương (PAY-FR-001).");
        }

        var probationReg = await regulations
            .FindByCodeAsync(PayRegulationCodes.ProbationTimeWageFactor, cancellationToken)
            .ConfigureAwait(false)
            ?? throw new InvalidOperationException(
                $"Thiếu master {PayRegulationCodes.ProbationTimeWageFactor}.");

        var employeeIds = tim.Lines.Select(l => l.EmployeeId).Distinct().ToList();
        var contracts = new Dictionary<Guid, EmployeeContractSnapshot?>();
        foreach (var id in employeeIds)
        {
            var emp = await employees.FindByIdAsync(id, cancellationToken).ConfigureAwait(false);
            contracts[id] = emp?.Contract;
        }

        var lines = tim.Lines.Select(l =>
        {
            contracts.TryGetValue(l.EmployeeId, out var contract);
            var factor = PayrollTimeWageFactor.Resolve(contract, ym, probationReg.DecimalValue);
            var nTinh = PayrollDayCalculator.ComputeNTinh(l.WorkDays, l.LeaveDaysUnpaid);
            // OT chỉ từ TIM Closed — PAY-FR-004 (không nhập tay).
            return new PayLineCreateModel(
                l.EmployeeId,
                l.EmployeeCode,
                l.WorkDays,
                l.LeaveDaysUnpaid,
                l.LeaveDaysPaid,
                nTinh,
                factor,
                l.Ot15,
                l.Ot20,
                l.Ot30);
        }).ToList();

        var period = await payPeriods
            .RunDraftAsync(ym, command.ActorIdpSubject!, lines, cancellationToken)
            .ConfigureAwait(false)
            ?? throw new ConflictException(
                HrmErrorCodes.Conflict,
                $"Kỳ PAY {ym} đã chốt — không chạy lại Draft.");

        return new PayRunResult(period.Id, period.PeriodYm, period.Status.ToString(), period.LineCount);
    }
}

public sealed record ClosePayrollPeriodCommand(string? ActorIdpSubject, string PeriodYm) : ICommand;

public sealed class ClosePayrollPeriodCommandHandler(
    IIdentityAccountReadRepository accounts,
    IPayPeriodRepository payPeriods)
    : IAsyncCommandHandler<ClosePayrollPeriodCommand, PayRunResult>
{
    public async Task<PayRunResult> HandleAsync(
        ClosePayrollPeriodCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);
        IamAccessGuard.RequireAuthenticated(command.ActorIdpSubject);
        var actor = await accounts.FindByIdpSubjectAsync(command.ActorIdpSubject!, cancellationToken)
            .ConfigureAwait(false);
        PayHrGuard.RequireHr(actor);

        if (string.IsNullOrWhiteSpace(command.PeriodYm)
            || command.PeriodYm.Length != 7
            || command.PeriodYm[4] != '-')
        {
            throw new BadRequestException(HrmErrorCodes.BadRequest, "PeriodYm phải dạng YYYY-MM.");
        }

        var ym = command.PeriodYm.Trim();
        await payPeriods.MarkClosedAsync(ym, command.ActorIdpSubject!, cancellationToken)
            .ConfigureAwait(false);

        var period = await payPeriods.FindByYmAsync(ym, cancellationToken).ConfigureAwait(false)
            ?? throw new InvalidOperationException("Pay period missing after close.");

        return new PayRunResult(period.Id, period.PeriodYm, period.Status.ToString(), period.LineCount);
    }
}
