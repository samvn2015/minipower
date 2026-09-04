using Hrm.Application.Lifecycle.Dtos;
using Hrm.Domain.Identity.Repositories;
using Hrm.Domain.Lifecycle.Repositories;
using Hrm.Domain.Shared.Constants;
using Jarvis.Application.Contracts.Queries;
using Jarvis.Domain.Shared.ExceptionHandling;
using Jarvis.Domain.Shared.Messaging;

namespace Hrm.Application.Lifecycle.Queries;

public sealed record ListLifOffboardingQuery(string ActorIdpSubject) : IQuery;

public sealed record GetLifOffboardingQuery(string ActorIdpSubject, Guid CaseId) : IQuery;

public sealed class ListLifOffboardingQueryHandler(
    IIdentityAccountReadRepository accounts,
    ILifOffboardingRepository offboardings)
    : IAsyncQueryHandler<ListLifOffboardingQuery, IReadOnlyList<LifOffboardingDto>>
{
    public async Task<IReadOnlyList<LifOffboardingDto>> HandleAsync(
        ListLifOffboardingQuery request,
        CancellationToken cancellationToken = default)
    {
        var actor = await accounts.FindByIdpSubjectAsync(request.ActorIdpSubject, cancellationToken)
            ?? throw new ForbiddenException(HrmErrorCodes.Forbidden, "Tài khoản không map.");
        LifAccessGuard.RequireHrOrPgd(actor);

        var rows = await offboardings.ListOpenAsync(cancellationToken);
        return rows.Select(LifOffboardingMapper.ToDto).ToList();
    }
}

public sealed class GetLifOffboardingQueryHandler(
    IIdentityAccountReadRepository accounts,
    ILifOffboardingRepository offboardings)
    : IAsyncQueryHandler<GetLifOffboardingQuery, LifOffboardingDto>
{
    public async Task<LifOffboardingDto> HandleAsync(
        GetLifOffboardingQuery request,
        CancellationToken cancellationToken = default)
    {
        var actor = await accounts.FindByIdpSubjectAsync(request.ActorIdpSubject, cancellationToken)
            ?? throw new ForbiddenException(HrmErrorCodes.Forbidden, "Tài khoản không map.");
        LifAccessGuard.RequireHrOrPgd(actor);

        var row = await offboardings.FindByIdAsync(request.CaseId, cancellationToken)
            ?? throw new NotFoundException(HrmErrorCodes.NotFound, "Không tìm thấy case offboarding.");
        return LifOffboardingMapper.ToDto(row);
    }
}
