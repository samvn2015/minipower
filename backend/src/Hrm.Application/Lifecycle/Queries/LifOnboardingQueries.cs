using Hrm.Application.Lifecycle.Commands;
using Hrm.Application.Lifecycle.Dtos;
using Hrm.Domain.Identity.Repositories;
using Hrm.Domain.Lifecycle.Repositories;
using Hrm.Domain.Shared.Constants;
using Jarvis.Application.Contracts.Queries;
using Jarvis.Domain.Shared.ExceptionHandling;
using Jarvis.Domain.Shared.Messaging;

using Hrm.Domain.Shared.Paging;

namespace Hrm.Application.Lifecycle.Queries;

/// <summary>S1 — danh sách onboarding tích luỹ theo mỗi lần tuyển, cần phân trang.</summary>
public sealed record ListLifOnboardingQuery(string ActorIdpSubject, int? Page = null, int? Size = null) : IQuery;

public sealed record GetLifOnboardingQuery(string ActorIdpSubject, Guid CaseId) : IQuery;

public sealed record GetLifOnChecklistQuery(string ActorIdpSubject, Guid CaseId) : IQuery;

public sealed class ListLifOnboardingQueryHandler(
    IIdentityAccountReadRepository accounts,
    ILifOnboardingRepository onboardings)
    : IAsyncQueryHandler<ListLifOnboardingQuery, PagedResult<LifOnboardingDto>>
{
    public async Task<PagedResult<LifOnboardingDto>> HandleAsync(
        ListLifOnboardingQuery request,
        CancellationToken cancellationToken = default)
    {
        var actor = await accounts.FindByIdpSubjectAsync(request.ActorIdpSubject, cancellationToken)
            ?? throw new ForbiddenException(HrmErrorCodes.Forbidden, "Tài khoản không map.");
        LifAccessGuard.RequireHrItOrPgd(actor);

        var paged = await onboardings.ListPagedAsync(
            PageRequest.From(request.Page, request.Size), cancellationToken);
        return new PagedResult<LifOnboardingDto>(
            paged.Items.Select(LifOnboardingMapper.ToDto).ToList(),
            paged.Total);
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
