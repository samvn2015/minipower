using Hrm.Application.Common;
using Hrm.Application.Payroll.Dtos;
using Hrm.Domain.Employees.Repositories;
using Hrm.Domain.Identity.Repositories;
using Hrm.Domain.Payroll;
using Hrm.Domain.Payroll.Repositories;
using Hrm.Domain.Shared.Constants;
using Jarvis.Application.Contracts.Commands;
using Jarvis.Domain.Shared.ExceptionHandling;

namespace Hrm.Application.Payroll.Commands;

/// <summary>Cấm sửa công trên PAY — luôn 400 (PAY-FR-008).</summary>
public sealed record RejectPayLineEditCommand(
    string? ActorIdpSubject,
    string PeriodYm,
    Guid LineId) : ICommand;

public sealed class RejectPayLineEditCommandHandler(
    IIdentityAccountReadRepository accounts)
    : IAsyncCommandHandler<RejectPayLineEditCommand, PayLineEditRejectedResult>
{
    public async Task<PayLineEditRejectedResult> HandleAsync(
        RejectPayLineEditCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);
        IamAccessGuard.RequireAuthenticated(command.ActorIdpSubject);
        var actor = await accounts.FindByIdpSubjectAsync(command.ActorIdpSubject!, cancellationToken)
            .ConfigureAwait(false);
        PayHrGuard.RequireHr(actor);

        throw new BadRequestException(
            HrmErrorCodes.BadRequest,
            "Cấm sửa ngày công / OT / phép trên PAY — sửa TIM rồi chạy lại / chốt lại (PAY-FR-008).");
    }
}

public sealed record ExportPayrollPeriodCommand(
    string? ActorIdpSubject,
    string PeriodYm,
    bool IncludePdf,
    bool IncludeEmail,
    IReadOnlyList<string>? CcAddresses) : ICommand;

public sealed class ExportPayrollPeriodCommandHandler(
    IIdentityAccountReadRepository accounts,
    IPayPeriodRepository payPeriods,
    IEmployeeReadRepository employees,
    IPayExportOutboxRepository outbox)
    : IAsyncCommandHandler<ExportPayrollPeriodCommand, PayExportResult>
{
    public async Task<PayExportResult> HandleAsync(
        ExportPayrollPeriodCommand command,
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

        if (!command.IncludePdf && !command.IncludeEmail)
        {
            throw new BadRequestException(
                HrmErrorCodes.BadRequest,
                "Cần chọn ít nhất PDF hoặc email (PAY-FR-012).");
        }

        try
        {
            PayExportRecipientGuard.EnsureNoCc(command.CcAddresses);
        }
        catch (InvalidOperationException ex) when (ex.Message == "CC_NOT_ALLOWED")
        {
            throw new BadRequestException(
                HrmErrorCodes.BadRequest,
                "Cấm CC LM hoặc địa chỉ phụ khi xuất phiếu (PAY-FR-012).");
        }

        var ym = command.PeriodYm.Trim();
        var period = await payPeriods.FindByYmAsync(ym, cancellationToken).ConfigureAwait(false)
            ?? throw new NotFoundException(HrmErrorCodes.NotFound, $"Không có kỳ PAY {ym}.");

        if (period.Status != PayPeriodStatus.Closed)
        {
            throw new BadRequestException(
                HrmErrorCodes.BadRequest,
                $"Kỳ {ym} chưa chốt — chỉ xuất khi Closed (PAY-FR-012).");
        }

        var items = new List<PayExportItemDto>(period.Lines.Count);
        var outboxRows = new List<PayExportOutboxCreateModel>();

        foreach (var line in period.Lines)
        {
            string? pdfFileName = null;
            string? pdfBase64 = null;
            string? toAddress = null;

            if (command.IncludePdf)
            {
                var bytes = PayPayslipPdfBuilder.Build(
                    ym,
                    line.EmployeeCode,
                    line.NTinh,
                    line.TimeWageFactor,
                    line.ContractAllowance,
                    line.MonthlyAllowance,
                    line.BhAmount,
                    line.TncnAmount,
                    line.NetPay);
                pdfFileName = $"payslip-{ym}-{line.EmployeeCode}.pdf";
                pdfBase64 = Convert.ToBase64String(bytes);
            }

            if (command.IncludeEmail)
            {
                var emp = await employees.FindByEmployeeCodeAsync(line.EmployeeCode, cancellationToken)
                    .ConfigureAwait(false);
                var email = emp?.EmailCty?.Trim();
                if (string.IsNullOrWhiteSpace(email))
                {
                    throw new BadRequestException(
                        HrmErrorCodes.BadRequest,
                        $"NV {line.EmployeeCode} thiếu EmailCty — cấm xuất email (PAY-FR-012).");
                }

                toAddress = email;
                try
                {
                    PayExportRecipientGuard.EnsureToMatchesEmployee(email, toAddress);
                }
                catch (InvalidOperationException ex) when (ex.Message == "TO_MISMATCH")
                {
                    throw new BadRequestException(
                        HrmErrorCodes.BadRequest,
                        $"To không khớp EmailCty của {line.EmployeeCode} (PAY-FR-012).");
                }

                outboxRows.Add(new PayExportOutboxCreateModel(
                    ym,
                    line.EmployeeCode,
                    toAddress,
                    CcAddress: null,
                    Channel: "Email",
                    Subject: $"Phiếu lương {ym} — {line.EmployeeCode}",
                    pdfFileName,
                    command.ActorIdpSubject!));
            }

            items.Add(new PayExportItemDto(
                line.EmployeeCode,
                toAddress,
                pdfFileName,
                pdfBase64));
        }

        if (outboxRows.Count > 0)
        {
            await outbox.AddManyAsync(outboxRows, cancellationToken).ConfigureAwait(false);
        }

        return new PayExportResult(
            ym,
            items.Count(i => i.PdfBase64 is not null),
            outboxRows.Count,
            items);
    }
}
