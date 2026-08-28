using Hrm.Application.Common;
using Hrm.Application.Employees.Dtos;
using Hrm.Domain.Employees.Repositories;
using Hrm.Domain.Identity.Repositories;
using Hrm.Domain.Shared.Constants;
using Jarvis.Application.Contracts.Queries;
using Jarvis.Domain.Shared.ExceptionHandling;

namespace Hrm.Application.Employees.Queries;

public sealed class ListEmployeesQueryHandler(
    IIdentityAccountReadRepository accounts,
    IEmployeeReadRepository employees)
    : IAsyncQueryHandler<ListEmployeesQuery, IReadOnlyList<EmployeeListItemDto>>
{
    public async Task<IReadOnlyList<EmployeeListItemDto>> HandleAsync(
        ListEmployeesQuery query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);
        IamAccessGuard.RequireAuthenticated(query.ActorIdpSubject);

        var actor = await accounts.FindByIdpSubjectAsync(query.ActorIdpSubject!, cancellationToken)
            .ConfigureAwait(false);
        if (actor is null || actor.Status != Domain.Identity.IdentityAccountStatus.Active)
            throw new ForbiddenException(HrmErrorCodes.Forbidden, "Tài khoản không hiệu lực.");

        if (!IamAccessGuard.IsHrOrIt(actor))
            throw new ForbiddenException(HrmErrorCodes.Forbidden, "Chỉ HR/IT xem danh sách NV (EMP-FR-012).");

        var items = await employees.ListAsync(cancellationToken).ConfigureAwait(false);
        return items.Select(static e => new EmployeeListItemDto(
            e.Id,
            e.EmployeeCode,
            e.FullName,
            e.EmailCty,
            e.Status.ToString())).ToArray();
    }
}
