using Hrm.Application.Common;
using Hrm.Application.Employees.Dtos;
using Hrm.Domain.Identity.Repositories;
using Hrm.Domain.Shared.Constants;
using Jarvis.Application.Contracts.Queries;
using Jarvis.Domain.Shared.ExceptionHandling;

namespace Hrm.Application.Employees.Queries;

/// <summary>EMP-SCR-006 — hàng chờ duyệt đổi LM.</summary>
public sealed record ListPendingLineManagerChangesQuery(string? ActorIdpSubject) : Jarvis.Domain.Shared.Messaging.IQuery;

public sealed class ListPendingLineManagerChangesQueryHandler(
    IIdentityAccountReadRepository accounts,
    Domain.Employees.Repositories.ILineManagerChangeRepository changes)
    : IAsyncQueryHandler<ListPendingLineManagerChangesQuery, IReadOnlyList<LineManagerChangeDto>>
{
    public async Task<IReadOnlyList<LineManagerChangeDto>> HandleAsync(
        ListPendingLineManagerChangesQuery query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);
        IamAccessGuard.RequireAuthenticated(query.ActorIdpSubject);

        var actor = await accounts.FindByIdpSubjectAsync(query.ActorIdpSubject!, cancellationToken)
            .ConfigureAwait(false);
        IamAccessGuard.RequireHrOrPgd(actor);

        var items = await changes.ListPendingAsync(cancellationToken).ConfigureAwait(false);
        return items.Select(LineManagerChangeDto.FromSnapshot).ToArray();
    }
}
