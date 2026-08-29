using Hrm.Application.Common;
using Hrm.Application.Employees;
using Hrm.Application.Employees.Dtos;
using Hrm.Domain.Employees.Repositories;
using Hrm.Domain.Identity.Repositories;
using Hrm.Domain.Shared.Constants;
using Jarvis.Application.Contracts.Queries;
using Jarvis.Domain.Shared.ExceptionHandling;

namespace Hrm.Application.Employees.Queries;

public sealed class GetMyEmployeeQueryHandler(
    IIdentityAccountReadRepository accounts,
    IEmployeeReadRepository employees)
    : IAsyncQueryHandler<GetMyEmployeeQuery, EmployeeDto>
{
    public async Task<EmployeeDto> HandleAsync(
        GetMyEmployeeQuery query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);
        IamAccessGuard.RequireAuthenticated(query.ActorIdpSubject);

        var actor = await accounts.FindByIdpSubjectAsync(query.ActorIdpSubject!, cancellationToken)
            .ConfigureAwait(false);
        if (actor is null || actor.Status != Domain.Identity.IdentityAccountStatus.Active)
            throw new ForbiddenException(HrmErrorCodes.Forbidden, "Tài khoản không hiệu lực.");

        if (string.IsNullOrWhiteSpace(actor.EmployeeCode))
        {
            throw new NotFoundException(
                HrmErrorCodes.NotFound,
                "Chưa liên kết MNV — không có hồ sơ EMP (IAM-FR-017).");
        }

        var employee = await employees
            .FindByEmployeeCodeAsync(actor.EmployeeCode, cancellationToken)
            .ConfigureAwait(false)
            ?? throw new NotFoundException(
                HrmErrorCodes.NotFound,
                $"Employee {actor.EmployeeCode} không tồn tại.");

        return EmployeeDtoMapper.Map(employee);
    }
}
