using Hrm.Application.Probation.Commands;
using Hrm.Application.Probation.Dtos;
using Hrm.Domain.Identity.Repositories;
using Hrm.Domain.Probation.Repositories;
using Hrm.Domain.Shared.Constants;
using Jarvis.Application.Contracts.Queries;
using Jarvis.Domain.Shared.ExceptionHandling;
using Jarvis.Domain.Shared.Messaging;

namespace Hrm.Application.Probation.Queries;

public sealed record ListProbationOutcomesQuery(string ActorIdpSubject) : IQuery;

public sealed record ListProbationCriteriaQuery(string ActorIdpSubject) : IQuery;

public sealed record ListProbationExtendDurationsQuery(string ActorIdpSubject) : IQuery;

public sealed record ListProbationEvaluationsQuery(string ActorIdpSubject) : IQuery;

public sealed class ListProbationOutcomesQueryHandler(
    IIdentityAccountReadRepository accounts,
    IProbationMasterReadRepository masters)
    : IAsyncQueryHandler<ListProbationOutcomesQuery, IReadOnlyList<ProbationMasterItemDto>>
{
    public async Task<IReadOnlyList<ProbationMasterItemDto>> HandleAsync(
        ListProbationOutcomesQuery request,
        CancellationToken cancellationToken = default)
    {
        await RequireAuth(accounts, request.ActorIdpSubject, cancellationToken);
        var rows = await masters.ListOutcomesAsync(cancellationToken);
        return rows.Select(x => new ProbationMasterItemDto(x.Code, x.Name, x.SortOrder)).ToList();
    }

    internal static async Task RequireAuth(
        IIdentityAccountReadRepository accounts,
        string subject,
        CancellationToken cancellationToken)
    {
        var actor = await accounts.FindByIdpSubjectAsync(subject, cancellationToken)
            ?? throw new ForbiddenException(HrmErrorCodes.Forbidden, "Tài khoản không map.");
        PrbAccessGuard.RequireAuthenticated(actor);
    }
}

public sealed class ListProbationCriteriaQueryHandler(
    IIdentityAccountReadRepository accounts,
    IProbationMasterReadRepository masters)
    : IAsyncQueryHandler<ListProbationCriteriaQuery, IReadOnlyList<ProbationMasterItemDto>>
{
    public async Task<IReadOnlyList<ProbationMasterItemDto>> HandleAsync(
        ListProbationCriteriaQuery request,
        CancellationToken cancellationToken = default)
    {
        await ListProbationOutcomesQueryHandler.RequireAuth(accounts, request.ActorIdpSubject, cancellationToken);
        var rows = await masters.ListCriteriaAsync(cancellationToken);
        return rows.Select(x => new ProbationMasterItemDto(x.Code, x.Name, x.SortOrder)).ToList();
    }
}

public sealed class ListProbationExtendDurationsQueryHandler(
    IIdentityAccountReadRepository accounts,
    IProbationMasterReadRepository masters)
    : IAsyncQueryHandler<ListProbationExtendDurationsQuery, IReadOnlyList<ProbationExtendDurationDto>>
{
    public async Task<IReadOnlyList<ProbationExtendDurationDto>> HandleAsync(
        ListProbationExtendDurationsQuery request,
        CancellationToken cancellationToken = default)
    {
        await ListProbationOutcomesQueryHandler.RequireAuth(accounts, request.ActorIdpSubject, cancellationToken);
        var rows = await masters.ListExtendDurationsAsync(cancellationToken);
        return rows.Select(x => new ProbationExtendDurationDto(x.Code, x.Name, x.Months, x.SortOrder)).ToList();
    }
}

public sealed class ListProbationEvaluationsQueryHandler(
    IIdentityAccountReadRepository accounts,
    IProbationEvaluationRepository evaluations)
    : IAsyncQueryHandler<ListProbationEvaluationsQuery, IReadOnlyList<ProbationEvaluationDto>>
{
    public async Task<IReadOnlyList<ProbationEvaluationDto>> HandleAsync(
        ListProbationEvaluationsQuery request,
        CancellationToken cancellationToken = default)
    {
        var actor = await accounts.FindByIdpSubjectAsync(request.ActorIdpSubject, cancellationToken)
            ?? throw new ForbiddenException(HrmErrorCodes.Forbidden, "Tài khoản không map.");
        PrbAccessGuard.RequireHrOrPgd(actor);
        var rows = await evaluations.ListAsync(cancellationToken);
        return rows.Select(s => ProbationEvaluationMapper.ToDto(s)).ToList();
    }
}
