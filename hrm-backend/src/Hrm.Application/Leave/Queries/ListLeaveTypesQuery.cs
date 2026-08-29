using Hrm.Application.Leave.Dtos;
using Hrm.Domain.Leave.Repositories;
using Jarvis.Application.Contracts.Queries;
using Jarvis.Domain.Shared.Messaging;

namespace Hrm.Application.Leave.Queries;

public sealed record ListLeaveTypesQuery : IQuery;

public sealed class ListLeaveTypesQueryHandler(ILeaveTypeReadRepository leaveTypes)
    : IAsyncQueryHandler<ListLeaveTypesQuery, IReadOnlyList<LeaveTypeDto>>
{
    public async Task<IReadOnlyList<LeaveTypeDto>> HandleAsync(
        ListLeaveTypesQuery query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);
        var items = await leaveTypes.ListActiveAsync(cancellationToken).ConfigureAwait(false);
        return items
            .Select(x => new LeaveTypeDto(x.Code, x.Name, x.DeductsAnnualBalance))
            .ToList();
    }
}
