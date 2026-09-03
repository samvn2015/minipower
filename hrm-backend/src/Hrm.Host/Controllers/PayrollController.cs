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

    public sealed record UpsertCalendarRequest(decimal StandardWorkDays);
}
