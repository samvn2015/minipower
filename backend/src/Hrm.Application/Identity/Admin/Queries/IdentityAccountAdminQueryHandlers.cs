using Hrm.Application.Common;
using Hrm.Application.Identity.Admin.Dtos;
using Hrm.Domain.Identity;
using Hrm.Domain.Identity.Repositories;
using Hrm.Domain.Shared.Constants;
using Jarvis.Application.Contracts.Queries;
using Jarvis.Domain.Shared.ExceptionHandling;

namespace Hrm.Application.Identity.Admin.Queries;

public sealed class ListIdentityAccountsQueryHandler(
    IIdentityAccountReadRepository accounts,
    IIdentityAccountAdminRepository admin)
    : IAsyncQueryHandler<ListIdentityAccountsQuery, IReadOnlyList<IdentityAccountDto>>
{
    public async Task<IReadOnlyList<IdentityAccountDto>> HandleAsync(
        ListIdentityAccountsQuery query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);
        IamAccessGuard.RequireAuthenticated(query.ActorIdpSubject);

        var actor = await accounts.FindByIdpSubjectAsync(query.ActorIdpSubject!, cancellationToken)
            .ConfigureAwait(false);
        IamAccessGuard.RequireHrOrIt(actor);

        var items = await admin.ListAsync(cancellationToken).ConfigureAwait(false);
        return items.Select(Map).ToArray();
    }

    internal static IdentityAccountDto Map(IdentityAccountSnapshot snapshot) =>
        new(
            snapshot.AccountId,
            snapshot.IdpSubject,
            snapshot.DisplayName,
            snapshot.EmailCty,
            snapshot.EmployeeCode,
            snapshot.Status.ToString(),
            snapshot.RoleCodes);
}

public sealed class GetIdentityAccountQueryHandler(
    IIdentityAccountReadRepository accounts,
    IIdentityAccountAdminRepository admin)
    : IAsyncQueryHandler<GetIdentityAccountQuery, IdentityAccountDto>
{
    public async Task<IdentityAccountDto> HandleAsync(
        GetIdentityAccountQuery query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);
        IamAccessGuard.RequireAuthenticated(query.ActorIdpSubject);

        var actor = await accounts.FindByIdpSubjectAsync(query.ActorIdpSubject!, cancellationToken)
            .ConfigureAwait(false);
        IamAccessGuard.RequireHrOrIt(actor);

        var account = await admin.FindByIdAsync(query.AccountId, cancellationToken).ConfigureAwait(false)
            ?? throw new NotFoundException(HrmErrorCodes.NotFound, $"IdentityAccount {query.AccountId} không tồn tại.");

        return ListIdentityAccountsQueryHandler.Map(account);
    }
}
