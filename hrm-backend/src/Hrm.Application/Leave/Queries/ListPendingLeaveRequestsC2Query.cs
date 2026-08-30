using Hrm.Application.Common;
using Hrm.Application.Leave.Dtos;
using Hrm.Domain.Identity.Repositories;
using Hrm.Domain.Leave.Repositories;
using Jarvis.Application.Contracts.Queries;
using Jarvis.Domain.Shared.Messaging;

namespace Hrm.Application.Leave.Queries;

public sealed record ListPendingLeaveRequestsC2Query(string? ActorIdpSubject) : IQuery;

public sealed class ListPendingLeaveRequestsC2QueryHandler(
    IIdentityAccountReadRepository accounts,
    ILeaveRequestRepository requests)
    : IAsyncQueryHandler<ListPendingLeaveRequestsC2Query, IReadOnlyList<LeaveRequestPendingC1Dto>>
{
    public async Task<IReadOnlyList<LeaveRequestPendingC1Dto>> HandleAsync(
        ListPendingLeaveRequestsC2Query query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);
        IamAccessGuard.RequireAuthenticated(query.ActorIdpSubject);

        var actor = await accounts.FindByIdpSubjectAsync(query.ActorIdpSubject!, cancellationToken)
            .ConfigureAwait(false);
        LevHrGuard.RequireHrForC2(actor);

        var items = await requests.ListPendingC2Async(cancellationToken).ConfigureAwait(false);

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
