using Hrm.Application.Common;
using Hrm.Application.Employees.Dtos;
using Hrm.Domain.Employees.Repositories;
using Hrm.Domain.Identity.Repositories;
using Hrm.Domain.Shared.Constants;
using Jarvis.Application.Contracts.Queries;
using Jarvis.Domain.Shared.ExceptionHandling;

namespace Hrm.Application.Employees.Queries;

public sealed record ListEducationLevelsQuery : Jarvis.Domain.Shared.Messaging.IQuery;

public sealed class ListEducationLevelsQueryHandler(IEducationLevelReadRepository educationLevels)
    : IAsyncQueryHandler<ListEducationLevelsQuery, IReadOnlyList<EducationLevelDto>>
{
    public async Task<IReadOnlyList<EducationLevelDto>> HandleAsync(
        ListEducationLevelsQuery query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);
        var items = await educationLevels.ListActiveAsync(cancellationToken).ConfigureAwait(false);
        return items.Select(x => new EducationLevelDto(x.Code, x.Name)).ToArray();
    }
}

public sealed record ListEmployeeAuditLogsQuery(string? ActorIdpSubject, Guid EmployeeId)
    : Jarvis.Domain.Shared.Messaging.IQuery;

public sealed class ListEmployeeAuditLogsQueryHandler(
    IIdentityAccountReadRepository accounts,
    IEmpAuditLogRepository auditLogs)
    : IAsyncQueryHandler<ListEmployeeAuditLogsQuery, IReadOnlyList<EmpAuditLogDto>>
{
    public async Task<IReadOnlyList<EmpAuditLogDto>> HandleAsync(
        ListEmployeeAuditLogsQuery query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);
        IamAccessGuard.RequireAuthenticated(query.ActorIdpSubject);

        var actor = await accounts.FindByIdpSubjectAsync(query.ActorIdpSubject!, cancellationToken)
            .ConfigureAwait(false);
        if (actor is null || actor.Status != Domain.Identity.IdentityAccountStatus.Active)
            throw new ForbiddenException(HrmErrorCodes.Forbidden, "Tài khoản không hiệu lực.");

        if (!IamAccessGuard.IsHrOrIt(actor))
            throw new ForbiddenException(HrmErrorCodes.Forbidden, "Chỉ HR/IT xem audit EMP.");

        var items = await auditLogs.ListByEmployeeIdAsync(query.EmployeeId, cancellationToken)
            .ConfigureAwait(false);
        return items.Select(x => new EmpAuditLogDto(
            x.Id,
            x.Action,
            x.EmployeeId,
            x.RelatedId,
            x.ActorIdpSubject,
            x.OccurredAtUtc,
            x.Detail)).ToArray();
    }
}
