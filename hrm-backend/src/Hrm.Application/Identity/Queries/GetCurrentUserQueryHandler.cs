using Hrm.Application.Identity.Dtos;
using Hrm.Domain.Identity;
using Hrm.Domain.Identity.Repositories;
using Jarvis.Application.Contracts.Queries;

namespace Hrm.Application.Identity.Queries;

public sealed class GetCurrentUserQueryHandler(IIdentityAccountReadRepository accounts)
    : IAsyncQueryHandler<GetCurrentUserQuery, CurrentUserDto>
{
    public async Task<CurrentUserDto> HandleAsync(
        GetCurrentUserQuery query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);
        cancellationToken.ThrowIfCancellationRequested();

        if (string.IsNullOrWhiteSpace(query.Subject))
        {
            return new CurrentUserDto(
                Sub: query.Subject,
                Name: query.Name,
                Roles: [],
                Note: "Thiếu claim sub — không map IAM DB (ADR-002).");
        }

        var account = await accounts.FindByIdpSubjectAsync(query.Subject, cancellationToken)
            .ConfigureAwait(false);

        if (account is null)
        {
            return new CurrentUserDto(
                Sub: query.Subject,
                Name: query.Name,
                Roles: [],
                Note: "Chưa có IdentityAccount trong IAM DB (SoT ADR-002).");
        }

        if (account.Status == IdentityAccountStatus.Disabled)
        {
            return new CurrentUserDto(
                Sub: query.Subject,
                Name: account.DisplayName ?? query.Name,
                Roles: [],
                Note: "Tài khoản vô hiệu (IAM-FR-010).");
        }

        var roles = account.RoleCodes
            .Where(static r => !string.IsNullOrWhiteSpace(r))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        return new CurrentUserDto(
            Sub: query.Subject,
            Name: account.DisplayName ?? query.Name,
            Roles: roles,
            Note: null);
    }
}
