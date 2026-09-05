using Hrm.Application.Common;
using Hrm.Application.Leave.Dtos;
using Hrm.Domain.Employees.Repositories;
using Hrm.Domain.Identity.Repositories;
using Hrm.Domain.Leave.Repositories;
using Jarvis.Application.Contracts.Queries;
using Jarvis.Domain.Shared.Messaging;

namespace Hrm.Application.Leave.Queries;

public sealed record ListMyLeaveNotificationsQuery(string? ActorIdpSubject) : IQuery;

public sealed class ListMyLeaveNotificationsQueryHandler(
    IIdentityAccountReadRepository accounts,
    IEmployeeReadRepository employees,
    ILeaveNotificationOutbox notifications)
    : IAsyncQueryHandler<ListMyLeaveNotificationsQuery, IReadOnlyList<LeaveNotificationDto>>
{
    public async Task<IReadOnlyList<LeaveNotificationDto>> HandleAsync(
        ListMyLeaveNotificationsQuery query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);
        var (_, employee) = await LevEmployeeGuard
            .ResolveActorEmployeeAsync(accounts, employees, query.ActorIdpSubject, cancellationToken)
            .ConfigureAwait(false);

        var rows = await notifications
            .ListByEmployeeAsync(employee.Id, cancellationToken)
            .ConfigureAwait(false);

        return rows
            .Select(r => new LeaveNotificationDto(
                r.Id,
                r.LeaveRequestId,
                r.EventType,
                r.Channel,
                r.Message,
                r.CreatedAtUtc.ToString("O")))
            .ToList();
    }
}
