using Hrm.Application.Common;
using Hrm.Application.Timekeeping.Dtos;
using Hrm.Domain.Employees.Repositories;
using Hrm.Domain.Identity.Repositories;
using Hrm.Domain.Leave.Repositories;
using Hrm.Domain.Payroll.Repositories;
using Hrm.Domain.Shared.Constants;
using Hrm.Domain.Timekeeping;
using Hrm.Domain.Timekeeping.Repositories;
using Jarvis.Application.Contracts.Commands;
using Jarvis.Domain.Shared.ExceptionHandling;
using Jarvis.Domain.Shared.Messaging;

namespace Hrm.Application.Timekeeping.Commands;

public sealed record PreviewTimesheetImportCommand(
    string? ActorIdpSubject,
    string PeriodYm,
    string TemplateVersionCode,
    string? FileName,
    IReadOnlyList<TimesheetImportRowValidator.RawImportRow> Rows) : ICommand;

public sealed class PreviewTimesheetImportCommandHandler(
    IIdentityAccountReadRepository accounts,
    ITimesheetTemplateRepository templates,
    IEmployeeReadRepository employees,
    ITimesheetImportRepository imports)
    : IAsyncCommandHandler<PreviewTimesheetImportCommand, TimesheetImportBatchDto>
{
    public async Task<TimesheetImportBatchDto> HandleAsync(
        PreviewTimesheetImportCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);
        IamAccessGuard.RequireAuthenticated(command.ActorIdpSubject);
        var actor = await accounts.FindByIdpSubjectAsync(command.ActorIdpSubject!, cancellationToken)
            .ConfigureAwait(false);
        TimHrGuard.RequireHrForImport(actor);

        if (string.IsNullOrWhiteSpace(command.PeriodYm)
            || command.PeriodYm.Length != 7
            || command.PeriodYm[4] != '-')
        {
            throw new BadRequestException(HrmErrorCodes.BadRequest, "PeriodYm phải dạng YYYY-MM.");
        }

        if (!string.IsNullOrWhiteSpace(command.FileName))
        {
            var name = command.FileName.Trim();
            var lower = name.ToLowerInvariant();
            if (lower.Contains("punch", StringComparison.Ordinal)
                || lower.Contains("device", StringComparison.Ordinal)
                || lower.Contains("zkteco", StringComparison.Ordinal))
            {
                throw new BadRequestException(
                    HrmErrorCodes.BadRequest,
                    "Chỉ nhận file Excel/CSV đúng mẫu — cấm máy CC / punch / device (TIM-FR-010 / TIM-TC-010).");
            }

            var ext = Path.GetExtension(name).ToLowerInvariant();
            if (ext is not (".xlsx" or ".xls" or ".csv"))
            {
                throw new BadRequestException(
                    HrmErrorCodes.BadRequest,
                    "Chỉ nhận .xlsx/.xls/.csv đúng mẫu — cấm máy CC (TIM-FR-010 / TIM-TC-010).");
            }
        }

        var active = await templates.FindActiveAsync(cancellationToken).ConfigureAwait(false)
            ?? throw new BadRequestException(HrmErrorCodes.BadRequest, "Chưa có mẫu Active.");

        if (!string.Equals(active.VersionCode, command.TemplateVersionCode.Trim(), StringComparison.Ordinal))
        {
            throw new BadRequestException(
                HrmErrorCodes.BadRequest,
                $"File không khớp mẫu hiệu lực {active.VersionCode} (TIM-FR-003).");
        }

        if (command.Rows.Count == 0)
            throw new BadRequestException(HrmErrorCodes.BadRequest, "File không có dòng dữ liệu.");

        var validated = await TimesheetImportRowValidator
            .ValidateAsync(command.Rows, employees, cancellationToken)
            .ConfigureAwait(false);

        var batchId = await imports.CreatePreviewAsync(
            new TimesheetImportBatchCreateModel(
                command.PeriodYm.Trim(),
                active.Id,
                active.VersionCode,
                command.ActorIdpSubject!,
                command.FileName,
                validated),
            cancellationToken).ConfigureAwait(false);

        var batch = await imports.FindBatchByIdAsync(batchId, cancellationToken).ConfigureAwait(false)
            ?? throw new InvalidOperationException("Preview batch missing after create.");

        return TimImportDtoMapper.Map(batch);
    }
}

public sealed record CommitTimesheetImportCommand(string? ActorIdpSubject, Guid BatchId) : ICommand;

public sealed class CommitTimesheetImportCommandHandler(
    IIdentityAccountReadRepository accounts,
    ITimesheetImportRepository imports)
    : IAsyncCommandHandler<CommitTimesheetImportCommand, TimesheetCommitResult>
{
    public async Task<TimesheetCommitResult> HandleAsync(
        CommitTimesheetImportCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);
        IamAccessGuard.RequireAuthenticated(command.ActorIdpSubject);
        var actor = await accounts.FindByIdpSubjectAsync(command.ActorIdpSubject!, cancellationToken)
            .ConfigureAwait(false);
        TimHrGuard.RequireHrForImport(actor);

        var batch = await imports.FindBatchByIdAsync(command.BatchId, cancellationToken).ConfigureAwait(false)
            ?? throw new NotFoundException(HrmErrorCodes.NotFound, "Batch import không tồn tại.");

        if (batch.Status != TimesheetImportBatchStatus.Preview)
            throw new ConflictException(HrmErrorCodes.Conflict, "Batch đã commit.");

        if (batch.HasMustErrors)
        {
            throw new BadRequestException(
                HrmErrorCodes.BadRequest,
                "Còn lỗi Must — cấm commit (TIM-FR-004/005).");
        }

        var period = await imports
            .CommitAsync(command.BatchId, command.ActorIdpSubject!, cancellationToken)
            .ConfigureAwait(false)
            ?? throw new BadRequestException(
                HrmErrorCodes.BadRequest,
                "Không commit được (kỳ đã chốt hoặc batch không hợp lệ).");

        return new TimesheetCommitResult(
            period.Id,
            period.PeriodYm,
            period.Status.ToString(),
            period.LineCount);
    }
}

public sealed record CloseTimesheetPeriodCommand(string? ActorIdpSubject, string PeriodYm) : ICommand;

public sealed class CloseTimesheetPeriodCommandHandler(
    IIdentityAccountReadRepository accounts,
    ITimesheetImportRepository imports,
    ILeaveRequestRepository leaveRequests)
    : IAsyncCommandHandler<CloseTimesheetPeriodCommand, TimesheetCloseResult>
{
    public async Task<TimesheetCloseResult> HandleAsync(
        CloseTimesheetPeriodCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);
        IamAccessGuard.RequireAuthenticated(command.ActorIdpSubject);
        var actor = await accounts.FindByIdpSubjectAsync(command.ActorIdpSubject!, cancellationToken)
            .ConfigureAwait(false);
        TimHrGuard.RequireHrForImport(actor);

        if (string.IsNullOrWhiteSpace(command.PeriodYm)
            || command.PeriodYm.Length != 7
            || command.PeriodYm[4] != '-')
        {
            throw new BadRequestException(HrmErrorCodes.BadRequest, "PeriodYm phải dạng YYYY-MM.");
        }

        var ym = command.PeriodYm.Trim();
        var period = await imports.FindPeriodByYmAsync(ym, cancellationToken).ConfigureAwait(false)
            ?? throw new NotFoundException(HrmErrorCodes.NotFound, $"Kỳ công {ym} không tồn tại.");

        if (period.Status != TimesheetPeriodStatus.Draft)
        {
            throw new ConflictException(
                HrmErrorCodes.Conflict,
                $"Kỳ {ym} không ở trạng thái Draft (hiện: {period.Status}).");
        }

        if (period.Lines.Any(l => l.OtUnclassified > 0))
        {
            throw new BadRequestException(
                HrmErrorCodes.BadRequest,
                "Còn giờ OT chưa phân loại 1.5/2.0/3.0 — cấm chốt (TIM-FR-007).");
        }

        var employeeIds = period.Lines.Select(l => l.EmployeeId).Distinct().ToList();
        var approved = await leaveRequests
            .ListApprovedOverlappingPeriodAsync(ym, employeeIds, cancellationToken)
            .ConfigureAwait(false);

        var leaveInputs = approved.Select(a => new ApprovedLeaveInput(
            a.EmployeeId,
            a.LeaveTypeCode,
            a.DeductsAnnualBalance,
            a.FromDate,
            a.ToDate,
            a.TotalDays)).ToList();

        var mergeLines = TimesheetLeaveMerger.BuildMergeLines(ym, employeeIds, leaveInputs);

        var closed = await imports
            .ClosePeriodAsync(ym, command.ActorIdpSubject!, mergeLines, cancellationToken)
            .ConfigureAwait(false)
            ?? throw new ConflictException(HrmErrorCodes.Conflict, $"Không chốt được kỳ {ym}.");

        return new TimesheetCloseResult(
            closed.Id,
            closed.PeriodYm,
            closed.Status.ToString(),
            closed.LineCount,
            closed.Lines.Sum(l => l.LeaveDaysPaid),
            closed.Lines.Sum(l => l.LeaveDaysUnpaid));
    }
}

public sealed record UnlockTimesheetPeriodCommand(string? ActorIdpSubject, string PeriodYm) : ICommand;

public sealed class UnlockTimesheetPeriodCommandHandler(
    IIdentityAccountReadRepository accounts,
    ITimesheetImportRepository imports,
    IPayPeriodGate payPeriods)
    : IAsyncCommandHandler<UnlockTimesheetPeriodCommand, TimesheetUnlockResult>
{
    public async Task<TimesheetUnlockResult> HandleAsync(
        UnlockTimesheetPeriodCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);
        IamAccessGuard.RequireAuthenticated(command.ActorIdpSubject);
        var actor = await accounts.FindByIdpSubjectAsync(command.ActorIdpSubject!, cancellationToken)
            .ConfigureAwait(false);
        TimHrGuard.RequireHrForImport(actor);

        if (string.IsNullOrWhiteSpace(command.PeriodYm)
            || command.PeriodYm.Length != 7
            || command.PeriodYm[4] != '-')
        {
            throw new BadRequestException(HrmErrorCodes.BadRequest, "PeriodYm phải dạng YYYY-MM.");
        }

        var ym = command.PeriodYm.Trim();
        var period = await imports.FindPeriodByYmAsync(ym, cancellationToken).ConfigureAwait(false)
            ?? throw new NotFoundException(HrmErrorCodes.NotFound, $"Kỳ công {ym} không tồn tại.");

        if (period.Status != TimesheetPeriodStatus.Closed)
        {
            throw new ConflictException(
                HrmErrorCodes.Conflict,
                $"Kỳ {ym} không ở trạng thái Closed (hiện: {period.Status}).");
        }

        if (await payPeriods.IsClosedAsync(ym, cancellationToken).ConfigureAwait(false))
        {
            throw new ConflictException(
                HrmErrorCodes.Conflict,
                $"Kỳ PAY {ym} đã chốt — cấm bỏ chốt TIM (TIM-FR-012 / TIM-BR-011).");
        }

        var unlocked = await imports
            .UnlockPeriodAsync(ym, cancellationToken)
            .ConfigureAwait(false)
            ?? throw new ConflictException(HrmErrorCodes.Conflict, $"Không bỏ chốt được kỳ {ym}.");

        return new TimesheetUnlockResult(
            unlocked.Id,
            unlocked.PeriodYm,
            unlocked.Status.ToString(),
            unlocked.LineCount);
    }
}

internal static class TimImportDtoMapper
{
    public static TimesheetImportBatchDto Map(TimesheetImportBatchSnapshot batch) =>
        new(
            batch.Id,
            batch.PeriodYm,
            batch.TemplateVersionCode,
            batch.Status.ToString(),
            batch.TotalRows,
            batch.ErrorRows,
            batch.HasMustErrors,
            batch.FileName,
            batch.Rows.Select(r => new TimesheetImportRowDto(
                r.RowNumber,
                r.EmployeeCode,
                r.WorkDays,
                r.Ot15,
                r.Ot20,
                r.Ot30,
                r.OtUnclassified,
                r.IsOk,
                r.ErrorCode,
                r.ErrorMessage)).ToList());

    public static TimesheetPeriodDto MapPeriod(TimesheetPeriodSnapshot period) =>
        new(
            period.Id,
            period.PeriodYm,
            period.Status.ToString(),
            period.SourceImportBatchId,
            period.LineCount,
            period.Lines.Select(l => new TimesheetLineDto(
                l.Id,
                l.EmployeeId,
                l.EmployeeCode,
                l.WorkDays,
                l.Ot15,
                l.Ot20,
                l.Ot30,
                l.OtUnclassified,
                l.LeaveDaysPaid,
                l.LeaveDaysUnpaid,
                l.LeaveDaysOther)).ToList());
}
