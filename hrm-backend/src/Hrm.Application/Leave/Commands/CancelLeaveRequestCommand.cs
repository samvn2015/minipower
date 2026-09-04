using Hrm.Application.Leave.Dtos;
using Hrm.Domain.Employees.Repositories;
using Hrm.Domain.Identity.Repositories;
using Hrm.Domain.Leave;
using Hrm.Domain.Leave.Repositories;
using Hrm.Domain.Shared.Constants;
using Jarvis.Application.Contracts.Commands;
using Jarvis.Domain.Shared.ExceptionHandling;
using Jarvis.Domain.Shared.Messaging;

namespace Hrm.Application.Leave.Commands;

public sealed record CancelLeaveRequestCommand(string? ActorIdpSubject, Guid RequestId) : ICommand;

public sealed class CancelLeaveRequestCommandHandler(
    IIdentityAccountReadRepository accounts,
    IEmployeeReadRepository employees,
    ILeaveRequestRepository requests,
    ILeaveNotificationOutbox notifications)
    : IAsyncCommandHandler<CancelLeaveRequestCommand, LeaveRequestActionResult>
{
    public async Task<LeaveRequestActionResult> HandleAsync(
        CancelLeaveRequestCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);
        var (_, employee) = await LevEmployeeGuard
            .ResolveActorEmployeeAsync(accounts, employees, command.ActorIdpSubject, cancellationToken)
            .ConfigureAwait(false);

        var request = await requests.FindByIdAsync(command.RequestId, cancellationToken).ConfigureAwait(false)
            ?? throw new NotFoundException(HrmErrorCodes.NotFound, "Đơn nghỉ phép không tồn tại.");

        if (request.EmployeeId != employee.Id)
        {
            throw new ForbiddenException(
                HrmErrorCodes.Forbidden,
                "Chỉ NV sở hữu đơn được hủy (LEV-FR-013).");
        }

        if (request.Status is LeaveRequestStatus.Approved or LeaveRequestStatus.Rejected
            or LeaveRequestStatus.Cancelled)
        {
            throw new ConflictException(
                HrmErrorCodes.Conflict,
                "Không hủy/hoàn quỹ sau C2 hoặc đơn đã đóng (LEV-FR-014).");
        }

        if (request.Status is not (LeaveRequestStatus.PendingC1 or LeaveRequestStatus.PendingC2))
        {
            throw new ConflictException(HrmErrorCodes.Conflict, "Đơn không thể hủy ở trạng thái hiện tại.");
        }

        var cancelled = await requests
            .CancelByEmployeeAsync(command.RequestId, employee.Id, cancellationToken)
            .ConfigureAwait(false);
        if (!cancelled)
            throw new NotFoundException(HrmErrorCodes.NotFound, "Không hủy được đơn.");

        await LeaveNotify.EmitAsync(
                notifications,
                request.Id,
                employee.Id,
                LeaveNotificationEvents.Cancelled,
                cancellationToken)
            .ConfigureAwait(false);

        return new LeaveRequestActionResult(command.RequestId, LeaveRequestStatus.Cancelled.ToString());
    }
}
