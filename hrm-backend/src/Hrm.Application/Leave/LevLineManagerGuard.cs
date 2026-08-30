using Hrm.Domain.Employees.Repositories;
using Hrm.Domain.Identity.Repositories;
using Hrm.Domain.Leave.Repositories;
using Hrm.Domain.Shared.Constants;
using Jarvis.Domain.Shared.ExceptionHandling;

namespace Hrm.Application.Leave;

public static class LevLineManagerGuard
{
    public static async Task<(IdentityAccountSnapshot Actor, EmployeeSnapshot Employee)> RequireLineManagerOfAsync(
        IIdentityAccountReadRepository accounts,
        IEmployeeReadRepository employees,
        ILeaveRequestRepository requests,
        string? actorIdpSubject,
        Guid leaveRequestId,
        CancellationToken cancellationToken)
    {
        var (actor, lmEmployee) = await LevEmployeeGuard
            .ResolveActorEmployeeAsync(accounts, employees, actorIdpSubject, cancellationToken)
            .ConfigureAwait(false);

        var request = await requests.FindByIdAsync(leaveRequestId, cancellationToken).ConfigureAwait(false)
            ?? throw new NotFoundException(HrmErrorCodes.NotFound, "Đơn nghỉ phép không tồn tại.");

        if (request.EmployeeId == lmEmployee.Id)
        {
            throw new ForbiddenException(
                HrmErrorCodes.Forbidden,
                "NV không được tự duyệt C1 đơn của mình (LEV-TC-010).");
        }

        var requestEmployee = await employees
            .FindByIdAsync(request.EmployeeId, cancellationToken)
            .ConfigureAwait(false)
            ?? throw new NotFoundException(HrmErrorCodes.NotFound, "NV nộp đơn không tồn tại.");

        if (requestEmployee.LineManagerEmployeeId != lmEmployee.Id)
        {
            throw new ForbiddenException(
                HrmErrorCodes.Forbidden,
                "Chỉ Line Manager trực tiếp được duyệt C1 (LEV-FR-010).");
        }

        return (actor, lmEmployee);
    }
}
