using Hrm.Application.Payroll.Commands;
using Hrm.Application.Payroll.Dtos;
using Hrm.Application.Payroll.Queries;
using Hrm.Host.Extensions;
using Jarvis.Application.Contracts.Commands;
using Jarvis.Application.Contracts.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hrm.Host.Controllers;

/// <summary>PAY — run Draft từ TIM Closed (FR-001/002) + close gate TIM-FR-012.</summary>
[ApiController]
[Route("v1/pay")]
[Authorize]
public sealed class PayrollController(
    IAsyncQueryDispatcher queries,
    IAsyncCommandDispatcher commands) : ControllerBase
{
    [HttpGet("periods")]
    public async Task<IActionResult> List(CancellationToken cancellationToken)
    {
        var items = await queries.DispatchAsync<ListPayrollPeriodsQuery, IReadOnlyList<PayPeriodDto>>(
            new ListPayrollPeriodsQuery(User.GetIdpSubject()),
            cancellationToken);
        return Ok(items);
    }

    [HttpGet("periods/{ym}")]
    public async Task<IActionResult> Get(string ym, CancellationToken cancellationToken)
    {
        var dto = await queries.DispatchAsync<GetPayrollPeriodQuery, PayPeriodDto>(
            new GetPayrollPeriodQuery(User.GetIdpSubject(), ym),
            cancellationToken);
        return Ok(dto);
    }

    /// <summary>Tính kỳ Draft từ TIM đã chốt — PAY-FR-001/002.</summary>
    [HttpPost("periods/{ym}/run")]
    public async Task<IActionResult> Run(string ym, CancellationToken cancellationToken)
    {
        var result = await commands.DispatchAsync<RunPayrollPeriodCommand, PayRunResult>(
            new RunPayrollPeriodCommand(User.GetIdpSubject(), ym),
            cancellationToken);
        return Ok(result);
    }

    /// <summary>Chốt kỳ PAY — chặn N_tính > chuẩn (FR-007) + gate TIM-FR-012.</summary>
    [HttpPost("periods/{ym}/close")]
    public async Task<IActionResult> Close(string ym, CancellationToken cancellationToken)
    {
        var result = await commands.DispatchAsync<ClosePayrollPeriodCommand, PayRunResult>(
            new ClosePayrollPeriodCommand(User.GetIdpSubject(), ym),
            cancellationToken);
        return Ok(result);
    }

    /// <summary>Ghi ngày công chuẩn tháng (lịch Cty D-004).</summary>
    [HttpPut("calendar/{ym}")]
    public async Task<IActionResult> UpsertCalendar(
        string ym,
        [FromBody] UpsertCalendarRequest body,
        CancellationToken cancellationToken)
    {
        var result = await commands.DispatchAsync<UpsertPayWorkdayCalendarCommand, PayWorkdayCalendarResult>(
            new UpsertPayWorkdayCalendarCommand(User.GetIdpSubject(), ym, body.StandardWorkDays),
            cancellationToken);
        return Ok(result);
    }

    /// <summary>Master mã PC — PAY-FR-005/015.</summary>
    [HttpGet("allowance-catalog")]
    public async Task<IActionResult> ListCatalog(CancellationToken cancellationToken)
    {
        var items = await queries
            .DispatchAsync<ListPayAllowanceCatalogQuery, IReadOnlyList<PayAllowanceCatalogDto>>(
                new ListPayAllowanceCatalogQuery(User.GetIdpSubject()),
                cancellationToken);
        return Ok(items);
    }

    /// <summary>Danh sách PC nhập tháng — PAY-SCR-004.</summary>
    [HttpGet("monthly-allowances/{ym}")]
    public async Task<IActionResult> ListMonthly(string ym, CancellationToken cancellationToken)
    {
        var items = await queries
            .DispatchAsync<ListPayMonthlyAllowancesQuery, IReadOnlyList<PayMonthlyAllowanceDto>>(
                new ListPayMonthlyAllowancesQuery(User.GetIdpSubject(), ym),
                cancellationToken);
        return Ok(items);
    }

    /// <summary>Ghi PC tháng; mã phải ∈ master — PAY-FR-015.</summary>
    [HttpPost("monthly-allowances")]
    public async Task<IActionResult> UpsertMonthly(
        [FromBody] UpsertMonthlyAllowanceRequest body,
        CancellationToken cancellationToken)
    {
        var result = await commands.DispatchAsync<UpsertPayMonthlyAllowanceCommand, PayMonthlyAllowanceResult>(
            new UpsertPayMonthlyAllowanceCommand(
                User.GetIdpSubject(),
                body.PeriodYm,
                body.EmployeeCode,
                body.Code,
                body.Amount),
            cancellationToken);
        return Ok(result);
    }

    /// <summary>Phiếu của tôi (kỳ Closed) — PAY-FR-010 · SCR-005.</summary>
    [HttpGet("payslips/me")]
    public async Task<IActionResult> ListMyPayslips(CancellationToken cancellationToken)
    {
        var items = await queries.DispatchAsync<ListMyPayslipsQuery, IReadOnlyList<PayPayslipDto>>(
            new ListMyPayslipsQuery(User.GetIdpSubject()),
            cancellationToken);
        return Ok(items);
    }

    /// <summary>Phiếu theo id dòng — HR hoặc chính chủ; LM cấp dưới → 403.</summary>
    [HttpGet("payslips/{id:guid}")]
    public async Task<IActionResult> GetPayslip(Guid id, CancellationToken cancellationToken)
    {
        var dto = await queries.DispatchAsync<GetPayslipQuery, PayPayslipDto>(
            new GetPayslipQuery(User.GetIdpSubject(), id),
            cancellationToken);
        return Ok(dto);
    }

    /// <summary>Cấm sửa công trên PAY — luôn 400 (PAY-FR-008).</summary>
    [HttpPut("periods/{ym}/lines/{lineId:guid}")]
    public async Task<IActionResult> RejectLineEdit(
        string ym,
        Guid lineId,
        CancellationToken cancellationToken)
    {
        var result = await commands.DispatchAsync<RejectPayLineEditCommand, PayLineEditRejectedResult>(
            new RejectPayLineEditCommand(User.GetIdpSubject(), ym, lineId),
            cancellationToken);
        return Ok(result);
    }

    /// <summary>Xuất PDF/email hàng loạt — kỳ Closed; cấm CC (PAY-FR-012).</summary>
    [HttpPost("periods/{ym}/export")]
    public async Task<IActionResult> Export(
        string ym,
        [FromBody] ExportPayrollRequest body,
        CancellationToken cancellationToken)
    {
        var result = await commands.DispatchAsync<ExportPayrollPeriodCommand, PayExportResult>(
            new ExportPayrollPeriodCommand(
                User.GetIdpSubject(),
                ym,
                body.IncludePdf,
                body.IncludeEmail,
                body.CcAddresses),
            cancellationToken);
        return Ok(result);
    }

    public sealed record UpsertCalendarRequest(decimal StandardWorkDays);

    public sealed record UpsertMonthlyAllowanceRequest(
        string PeriodYm,
        string EmployeeCode,
        string Code,
        decimal Amount);

    public sealed record ExportPayrollRequest(
        bool IncludePdf,
        bool IncludeEmail,
        IReadOnlyList<string>? CcAddresses);
}
