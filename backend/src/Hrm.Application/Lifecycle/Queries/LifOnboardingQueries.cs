using Hrm.Application.Lifecycle.Commands;
using Hrm.Application.Lifecycle.Dtos;
using Hrm.Domain.Identity.Repositories;
using Hrm.Domain.Lifecycle.Repositories;
using Hrm.Domain.Shared.Constants;
using Jarvis.Application.Contracts.Queries;
using Jarvis.Domain.Shared.ExceptionHandling;
using Jarvis.Domain.Shared.Messaging;

namespace Hrm.Application.Lifecycle.Queries;

public sealed record ListLifOnboardingQuery(string ActorIdpSubject) : IQuery;

public sealed record GetLifOnboardingQuery(string ActorIdpSubject, Guid CaseId) : IQuery;

public sealed record GetLifOnChecklistQuery(string ActorIdpSubject, Guid CaseId) : IQuery;

public sealed class ListLifOnboardingQueryHandler(
    IIdentityAccountReadRepository accounts,
    ILifOnboardingRepository onboardings)
    : IAsyncQueryHandler<ListLifOnboardingQuery, IReadOnlyList<LifOnboardingDto>>
{
    public async Task<IReadOnlyList<LifOnboardingDto>> HandleAsync(
        ListLifOnboardingQuery request,
        CancellationToken cancellationToken = default)
    {
        var actor = await accounts.FindByIdpSubjectAsync(request.ActorIdpSubject, cancellationToken)
            ?? throw new ForbiddenException(HrmErrorCodes.Forbidden, "Tài khoản không map.");
        LifAccessGuard.RequireHrItOrPgd(actor);

        var rows = await onboardings.ListAsync(cancellationToken);
        return rows.Select(LifOnboardingMapper.ToDto).ToList();
    }
}

public sealed class GetLifOnboardingQueryHandler(
    IIdentityAccountReadRepository accounts,
    ILifOnboardingRepository onboardings)
    : IAsyncQueryHandler<GetLifOnboardingQuery, LifOnboardingDto>
{
    public async Task<LifOnboardingDto> HandleAsync(
        GetLifOnboardingQuery request,
        CancellationToken cancellationToken = default)
    {
        var actor = await accounts.FindByIdpSubjectAsync(request.ActorIdpSubject, cancellationToken)
            ?? throw new ForbiddenException(HrmErrorCodes.Forbidden, "Tài khoản không map.");
        LifAccessGuard.RequireHrItOrPgd(actor);

        var row = await onboardings.FindByIdAsync(request.CaseId, cancellationToken)
            ?? throw new NotFoundException(HrmErrorCodes.NotFound, "Không tìm thấy case onboarding.");
        return LifOnboardingMapper.ToDto(row);
    }
}

public sealed class GetLifOnChecklistQueryHandler(
    IIdentityAccountReadRepository accounts,
    ILifOnboardingRepository onboardings,
    ILifOnChecklistRepository checklist)
    : IAsyncQueryHandler<GetLifOnChecklistQuery, LifOffChecklistBoardDto>
{
    public async Task<LifOffChecklistBoardDto> HandleAsync(
        GetLifOnChecklistQuery request,
        CancellationToken cancellationToken = default)
    {
        var actor = await accounts.FindByIdpSubjectAsync(request.ActorIdpSubject, cancellationToken)
            ?? throw new ForbiddenException(HrmErrorCodes.Forbidden, "Tài khoản không map.");
        LifAccessGuard.RequireHrItOrPgd(actor);

        var row = await onboardings.FindByIdAsync(request.CaseId, cancellationToken)
            ?? throw new NotFoundException(HrmErrorCodes.NotFound, "Không tìm thấy case onboarding.");

        return await LifOnChecklistBoardBuilder.BuildAsync(checklist, row, cancellationToken);
    }
}
