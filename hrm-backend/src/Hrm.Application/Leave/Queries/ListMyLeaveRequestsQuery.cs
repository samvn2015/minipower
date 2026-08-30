using Hrm.Application.Leave.Dtos;
using Hrm.Domain.Employees.Repositories;
using Hrm.Domain.Identity.Repositories;
using Hrm.Domain.Leave.Repositories;
using Jarvis.Application.Contracts.Queries;
using Jarvis.Domain.Shared.Messaging;

namespace Hrm.Application.Leave.Queries;

public sealed record ListMyLeaveRequestsQuery(string? ActorIdpSubject) : IQuery;

public sealed class ListMyLeaveRequestsQueryHandler(
    IIdentityAccountReadRepository accounts,
    IEmployeeReadRepository employees,
    ILeaveRequestRepository requests)
    : IAsyncQueryHandler<ListMyLeaveRequestsQuery, IReadOnlyList<LeaveRequestDto>>
{
    public async Task<IReadOnlyList<LeaveRequestDto>> HandleAsync(
        ListMyLeaveRequestsQuery query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);
        var (_, employee) = await LevEmployeeGuard
            .ResolveActorEmployeeAsync(accounts, employees, query.ActorIdpSubject, cancellationToken)
            .ConfigureAwait(false);

        var items = await requests
            .ListByEmployeeIdAsync(employee.Id, cancellationToken)
            .ConfigureAwait(false);

        return items.Select(LeaveRequestDtoMapper.Map).ToList();
    }
}

internal static class LeaveRequestDtoMapper
{
    public static LeaveRequestDto Map(LeaveRequestSnapshot snapshot) =>
        new(
            snapshot.Id,
            snapshot.LeaveTypeCode,
            snapshot.LeaveTypeName,
            snapshot.FromDate.ToString("yyyy-MM-dd"),
            snapshot.ToDate.ToString("yyyy-MM-dd"),
            snapshot.DayPart.ToString(),
            snapshot.TotalDays,
            snapshot.Reason,
            snapshot.HandoverEmployeeId,
            snapshot.Status.ToString(),
            snapshot.IsEmergency);
}
