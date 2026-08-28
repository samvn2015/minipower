using Hrm.Application.Common;
using Hrm.Application.Employees.Dtos;
using Hrm.Domain.Employees.Repositories;
using Hrm.Domain.Identity.Repositories;
using Hrm.Domain.Shared.Constants;
using Jarvis.Application.Contracts.Queries;
using Jarvis.Domain.Shared.ExceptionHandling;

namespace Hrm.Application.Employees.Queries;

public sealed class GetEmployeeQueryHandler(
    IIdentityAccountReadRepository accounts,
    IEmployeeReadRepository employees)
    : IAsyncQueryHandler<GetEmployeeQuery, EmployeeDto>
{
    public async Task<EmployeeDto> HandleAsync(
        GetEmployeeQuery query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);
        IamAccessGuard.RequireAuthenticated(query.ActorIdpSubject);

        var actor = await accounts.FindByIdpSubjectAsync(query.ActorIdpSubject!, cancellationToken)
            .ConfigureAwait(false);
        if (actor is null || actor.Status != Domain.Identity.IdentityAccountStatus.Active)
            throw new ForbiddenException(HrmErrorCodes.Forbidden, "Tài khoản không hiệu lực.");

        var employee = await employees.FindByIdAsync(query.EmployeeId, cancellationToken).ConfigureAwait(false)
            ?? throw new NotFoundException(HrmErrorCodes.NotFound, $"Employee {query.EmployeeId} không tồn tại.");

        if (!IamAccessGuard.IsHrOrIt(actor))
        {
            if (string.IsNullOrWhiteSpace(actor.EmployeeCode)
                || !string.Equals(actor.EmployeeCode, employee.EmployeeCode, StringComparison.OrdinalIgnoreCase))
            {
                throw new ForbiddenException(HrmErrorCodes.Forbidden, "Chỉ xem hồ sơ của chính mình (EMP-FR-011).");
            }
        }

        return Map(employee);
    }

    internal static EmployeeDto Map(EmployeeSnapshot snapshot) =>
        new(
            snapshot.Id,
            snapshot.EmployeeCode,
            snapshot.FullName,
            snapshot.Cccd,
            snapshot.EmailCty,
            snapshot.TaxId,
            snapshot.LineManagerEmployeeId,
            snapshot.Status.ToString());
}
