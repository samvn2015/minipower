using Hrm.Application.Leave.Dtos;
using Hrm.Domain.Employees.Repositories;
using Hrm.Domain.Identity.Repositories;
using Hrm.Domain.Leave.Repositories;
using Hrm.Domain.Shared.Constants;
using Jarvis.Application.Contracts.Queries;
using Jarvis.Domain.Shared.ExceptionHandling;
using Jarvis.Domain.Shared.Messaging;

namespace Hrm.Application.Leave.Queries;

public sealed record GetMyLeaveBalanceQuery(string? ActorIdpSubject, int? Year) : IQuery;

public sealed class GetMyLeaveBalanceQueryHandler(
    IIdentityAccountReadRepository accounts,
    IEmployeeReadRepository employees,
    ILeaveBalanceRepository balances)
    : IAsyncQueryHandler<GetMyLeaveBalanceQuery, LeaveBalanceDto>
{
    public async Task<LeaveBalanceDto> HandleAsync(
        GetMyLeaveBalanceQuery query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);
        var (_, employee) = await LevEmployeeGuard
            .ResolveActorEmployeeAsync(accounts, employees, query.ActorIdpSubject, cancellationToken)
            .ConfigureAwait(false);

        var year = query.Year ?? DateTime.UtcNow.Year;
        var balance = await balances
            .FindByEmployeeAndYearAsync(employee.Id, year, cancellationToken)
            .ConfigureAwait(false)
            ?? throw new NotFoundException(
                HrmErrorCodes.NotFound,
                $"Chưa có quỹ phép năm {year} (LEV-FR-015).");

        return new LeaveBalanceDto(
            balance.Year,
            balance.EntitledDays,
            balance.UsedDays,
            balance.RemainingDays);
    }
}
