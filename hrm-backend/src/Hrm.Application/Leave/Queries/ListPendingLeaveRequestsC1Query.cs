using Hrm.Application.Leave.Dtos;
using Hrm.Domain.Employees.Repositories;
using Hrm.Domain.Identity.Repositories;
using Hrm.Domain.Leave.Repositories;
using Jarvis.Application.Contracts.Queries;
using Jarvis.Domain.Shared.Messaging;

namespace Hrm.Application.Leave.Queries;

public sealed record ListPendingLeaveRequestsC1Query(string? ActorIdpSubject) : IQuery;

public sealed class ListPendingLeaveRequestsC1QueryHandler(
    IIdentityAccountReadRepository accounts,
    IEmployeeReadRepository employees,
    ILeaveRequestRepository requests)
    : IAsyncQueryHandler<ListPendingLeaveRequestsC1Query, IReadOnlyList<LeaveRequestPendingC1Dto>>
{
    public async Task<IReadOnlyList<LeaveRequestPendingC1Dto>> HandleAsync(
        ListPendingLeaveRequestsC1Query query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);
        var (_, lmEmployee) = await LevEmployeeGuard
            .ResolveActorEmployeeAsync(accounts, employees, query.ActorIdpSubject, cancellationToken)
            .ConfigureAwait(false);

        var items = await requests
            .ListPendingC1ByLineManagerIdAsync(lmEmployee.Id, cancellationToken)
            .ConfigureAwait(false);

        return items.Select(static item => new LeaveRequestPendingC1Dto(
            item.Id,
            item.EmployeeCode,
            item.EmployeeFullName,
            item.LeaveTypeCode,
            item.LeaveTypeName,
            item.FromDate.ToString("yyyy-MM-dd"),
            item.ToDate.ToString("yyyy-MM-dd"),
            item.DayPart.ToString(),
            item.TotalDays,
            item.Reason,
            item.HandoverEmployeeId,
            item.IsEmergency,
            item.SubmittedAtUtc.ToString("O"))).ToList();
    }
}
