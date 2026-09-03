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
    IPayRegulationReadRepository regulations,
    IPayAllowanceRepository allowances,
    IPayContractSalaryRepository salaries)
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
        var bhReg = await regulations
            .FindByCodeAsync(PayRegulationCodes.BhEmployeeRate, cancellationToken)
            .ConfigureAwait(false)
            ?? throw new InvalidOperationException(
                $"Thiếu master {PayRegulationCodes.BhEmployeeRate}.");
        var tncnReg = await regulations
            .FindByCodeAsync(PayRegulationCodes.TncnTempRate, cancellationToken)
            .ConfigureAwait(false)
            ?? throw new InvalidOperationException(
                $"Thiếu master {PayRegulationCodes.TncnTempRate}.");

        var employeeIds = tim.Lines.Select(l => l.EmployeeId).Distinct().ToList();
        var contracts = new Dictionary<Guid, EmployeeContractSnapshot?>();
        foreach (var id in employeeIds)
        {
            var emp = await employees.FindByIdAsync(id, cancellationToken).ConfigureAwait(false);
            contracts[id] = emp?.Contract;
        }

        var lines = new List<PayLineCreateModel>(tim.Lines.Count);
        foreach (var l in tim.Lines)
        {
            contracts.TryGetValue(l.EmployeeId, out var contract);
            var factor = PayrollTimeWageFactor.Resolve(contract, ym, probationReg.DecimalValue);
            var nTinh = PayrollDayCalculator.ComputeNTinh(l.WorkDays, l.LeaveDaysUnpaid);
            var contractPc = await allowances
                .SumContractAsync(l.EmployeeId, cancellationToken)
                .ConfigureAwait(false);
            var monthlyPc = await allowances
                .SumMonthlyAsync(ym, l.EmployeeId, cancellationToken)
                .ConfigureAwait(false);
            var salary = await salaries
                .GetAmountAsync(l.EmployeeId, cancellationToken)
                .ConfigureAwait(false);
            var statutory = PayrollStatutoryCalculator.Compute(
                salary,
                factor,
                contractPc,
                monthlyPc,
                bhReg.DecimalValue,
                tncnReg.DecimalValue);
            // OT chỉ từ TIM Closed — PAY-FR-004 (không nhập tay).
            lines.Add(new PayLineCreateModel(
                l.EmployeeId,
                l.EmployeeCode,
                l.WorkDays,
                l.LeaveDaysUnpaid,
                l.LeaveDaysPaid,
                nTinh,
                factor,
                l.Ot15,
                l.Ot20,
                l.Ot30,
                contractPc,
                monthlyPc,
                statutory.BhRate,
                statutory.TncnRate,
                statutory.BhAmount,
                statutory.TncnAmount,
                statutory.NetPay));
        }

        var period = await payPeriods
            .RunDraftAsync(ym, command.ActorIdpSubject!, lines, cancellationToken)
            .ConfigureAwait(false)
            ?? throw new ConflictException(
                HrmErrorCodes.Conflict,
                $"Kỳ PAY {ym} đã chốt — không chạy lại Draft (PAY-FR-016).");

        return new PayRunResult(period.Id, period.PeriodYm, period.Status.ToString(), period.LineCount);
    }
}

public sealed record ClosePayrollPeriodCommand(string? ActorIdpSubject, string PeriodYm) : ICommand;

public sealed class ClosePayrollPeriodCommandHandler(
    IIdentityAccountReadRepository accounts,
    IPayPeriodRepository payPeriods,
    IPayRegulationReadRepository regulations,
    IPayWorkdayCalendarRepository calendar,
    IPayAllowanceRepository allowances)
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
        var unknownCodes = await allowances
            .ListUnknownMonthlyCodesAsync(ym, cancellationToken)
            .ConfigureAwait(false);
        if (unknownCodes.Count > 0)
        {
            throw new BadRequestException(
                HrmErrorCodes.BadRequest,
                $"Mã PC tháng không thuộc master (PAY-FR-005): {string.Join(", ", unknownCodes)}.");
        }

        var existing = await payPeriods.FindByYmAsync(ym, cancellationToken).ConfigureAwait(false);
        if (existing is { LineCount: > 0 })
        {
            var fallback = await regulations
                .FindByCodeAsync(PayRegulationCodes.StandardWorkDaysDefault, cancellationToken)
                .ConfigureAwait(false);
            var standard = await calendar
                .ResolveStandardWorkDaysAsync(ym, fallback?.DecimalValue ?? 22m, cancellationToken)
                .ConfigureAwait(false);

            var over = existing.Lines
                .Where(l => PayrollWorkdayCap.ExceedsCap(l.NTinh, standard))
                .Select(l => l.EmployeeCode)
                .ToList();
            if (over.Count > 0)
            {
                throw new BadRequestException(
                    HrmErrorCodes.BadRequest,
                    $"N_tính > ngày công chuẩn {standard} (PAY-FR-007): {string.Join(", ", over)}.");
            }
        }

        await payPeriods.MarkClosedAsync(ym, command.ActorIdpSubject!, cancellationToken)
            .ConfigureAwait(false);

        var period = await payPeriods.FindByYmAsync(ym, cancellationToken).ConfigureAwait(false)
            ?? throw new InvalidOperationException("Pay period missing after close.");

        return new PayRunResult(period.Id, period.PeriodYm, period.Status.ToString(), period.LineCount);
    }
}

public sealed record UpsertPayWorkdayCalendarCommand(
    string? ActorIdpSubject,
    string PeriodYm,
    decimal StandardWorkDays) : ICommand;

public sealed class UpsertPayWorkdayCalendarCommandHandler(
    IIdentityAccountReadRepository accounts,
    IPayWorkdayCalendarRepository calendar)
    : IAsyncCommandHandler<UpsertPayWorkdayCalendarCommand, PayWorkdayCalendarResult>
{
    public async Task<PayWorkdayCalendarResult> HandleAsync(
        UpsertPayWorkdayCalendarCommand command,
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

        if (command.StandardWorkDays <= 0 || command.StandardWorkDays > 31)
            throw new BadRequestException(HrmErrorCodes.BadRequest, "StandardWorkDays phải trong (0, 31].");

        var ym = command.PeriodYm.Trim();
        await calendar.UpsertAsync(ym, command.StandardWorkDays, cancellationToken).ConfigureAwait(false);
        return new PayWorkdayCalendarResult(ym, command.StandardWorkDays);
    }
}
