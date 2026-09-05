using Hrm.Application.Common;
using Hrm.Application.Payroll.Dtos;
using Hrm.Domain.Employees.Repositories;
using Hrm.Domain.Identity.Repositories;
using Hrm.Domain.Payroll.Repositories;
using Hrm.Domain.Shared.Constants;
using Jarvis.Application.Contracts.Commands;
using Jarvis.Application.Contracts.Queries;
using Jarvis.Domain.Shared.ExceptionHandling;
using Jarvis.Domain.Shared.Messaging;

namespace Hrm.Application.Payroll.Commands;

public sealed record UpsertPayMonthlyAllowanceCommand(
    string? ActorIdpSubject,
    string PeriodYm,
    string EmployeeCode,
    string Code,
    decimal Amount) : ICommand;

public sealed class UpsertPayMonthlyAllowanceCommandHandler(
    IIdentityAccountReadRepository accounts,
    IEmployeeReadRepository employees,
    IPayAllowanceRepository allowances,
    IPayPeriodRepository payPeriods)
    : IAsyncCommandHandler<UpsertPayMonthlyAllowanceCommand, PayMonthlyAllowanceResult>
{
    public async Task<PayMonthlyAllowanceResult> HandleAsync(
        UpsertPayMonthlyAllowanceCommand command,
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
        var code = (command.Code ?? "").Trim().ToUpperInvariant();
        var employeeCode = (command.EmployeeCode ?? "").Trim();

        if (string.IsNullOrWhiteSpace(employeeCode))
            throw new BadRequestException(HrmErrorCodes.BadRequest, "EmployeeCode bắt buộc.");

        if (string.IsNullOrWhiteSpace(code))
            throw new BadRequestException(HrmErrorCodes.BadRequest, "Mã PC bắt buộc.");

        if (command.Amount < 0)
            throw new BadRequestException(HrmErrorCodes.BadRequest, "Amount không được âm.");

        if (await payPeriods.IsClosedAsync(ym, cancellationToken).ConfigureAwait(false))
        {
            throw new ConflictException(
                HrmErrorCodes.Conflict,
                $"Kỳ PAY {ym} đã chốt — không nhập PC tháng.");
        }

        if (!await allowances.IsActiveCodeAsync(code, cancellationToken).ConfigureAwait(false))
        {
            throw new BadRequestException(
                HrmErrorCodes.BadRequest,
                $"Mã PC {code} không thuộc master kỳ (PAY-FR-015).");
        }

        var employee = await employees
            .FindByEmployeeCodeAsync(employeeCode, cancellationToken)
            .ConfigureAwait(false)
            ?? throw new NotFoundException(HrmErrorCodes.NotFound, $"Không tìm thấy NV {employeeCode}.");

        await allowances
            .UpsertMonthlyAsync(ym, employee.Id, employee.EmployeeCode, code, command.Amount, cancellationToken)
            .ConfigureAwait(false);

        return new PayMonthlyAllowanceResult(ym, employee.EmployeeCode, code, command.Amount);
    }
}

public sealed record ListPayAllowanceCatalogQuery(string? ActorIdpSubject) : IQuery;

public sealed class ListPayAllowanceCatalogQueryHandler(
    IIdentityAccountReadRepository accounts,
    IPayAllowanceRepository allowances)
    : IAsyncQueryHandler<ListPayAllowanceCatalogQuery, IReadOnlyList<PayAllowanceCatalogDto>>
{
    public async Task<IReadOnlyList<PayAllowanceCatalogDto>> HandleAsync(
        ListPayAllowanceCatalogQuery query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);
        IamAccessGuard.RequireAuthenticated(query.ActorIdpSubject);
        var actor = await accounts.FindByIdpSubjectAsync(query.ActorIdpSubject!, cancellationToken)
            .ConfigureAwait(false);
        PayHrGuard.RequireHr(actor);

        var rows = await allowances.ListCatalogAsync(cancellationToken).ConfigureAwait(false);
        return rows.Select(r => new PayAllowanceCatalogDto(r.Code, r.Name, r.IsActive)).ToList();
    }
}

public sealed record ListPayMonthlyAllowancesQuery(string? ActorIdpSubject, string PeriodYm) : IQuery;

public sealed class ListPayMonthlyAllowancesQueryHandler(
    IIdentityAccountReadRepository accounts,
    IPayAllowanceRepository allowances)
    : IAsyncQueryHandler<ListPayMonthlyAllowancesQuery, IReadOnlyList<PayMonthlyAllowanceDto>>
{
    public async Task<IReadOnlyList<PayMonthlyAllowanceDto>> HandleAsync(
        ListPayMonthlyAllowancesQuery query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);
        IamAccessGuard.RequireAuthenticated(query.ActorIdpSubject);
        var actor = await accounts.FindByIdpSubjectAsync(query.ActorIdpSubject!, cancellationToken)
            .ConfigureAwait(false);
        PayHrGuard.RequireHr(actor);

        if (string.IsNullOrWhiteSpace(query.PeriodYm)
            || query.PeriodYm.Length != 7
            || query.PeriodYm[4] != '-')
        {
            throw new BadRequestException(HrmErrorCodes.BadRequest, "PeriodYm phải dạng YYYY-MM.");
        }

        var rows = await allowances
            .ListMonthlyByYmAsync(query.PeriodYm.Trim(), cancellationToken)
            .ConfigureAwait(false);
        return rows.Select(r => new PayMonthlyAllowanceDto(
            r.Id, r.PeriodYm, r.EmployeeId, r.EmployeeCode, r.Code, r.Amount)).ToList();
    }
}
