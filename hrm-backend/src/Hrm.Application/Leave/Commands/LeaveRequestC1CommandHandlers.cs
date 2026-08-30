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

public sealed record ApproveLeaveRequestC1Command(string? ActorIdpSubject, Guid RequestId) : ICommand;

public sealed class ApproveLeaveRequestC1CommandHandler(
    IIdentityAccountReadRepository accounts,
    IEmployeeReadRepository employees,
    ILeaveRequestRepository requests)
    : IAsyncCommandHandler<ApproveLeaveRequestC1Command, LeaveRequestActionResult>
{
    public async Task<LeaveRequestActionResult> HandleAsync(
        ApproveLeaveRequestC1Command command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);
        await LevLineManagerGuard
            .RequireLineManagerOfAsync(
                accounts,
                employees,
                requests,
                command.ActorIdpSubject,
                command.RequestId,
                cancellationToken)
            .ConfigureAwait(false);

        var request = await requests.FindByIdAsync(command.RequestId, cancellationToken).ConfigureAwait(false)!;
        if (request!.Status != LeaveRequestStatus.PendingC1)
        {
            throw new ConflictException(HrmErrorCodes.Conflict, "Đơn không còn chờ duyệt C1.");
        }

        var approved = await requests
            .ApproveC1Async(command.RequestId, command.ActorIdpSubject!, cancellationToken)
            .ConfigureAwait(false);
        if (!approved)
            throw new NotFoundException(HrmErrorCodes.NotFound, "Không duyệt C1 được đơn.");

        return new LeaveRequestActionResult(command.RequestId, LeaveRequestStatus.PendingC2.ToString());
    }
}

public sealed record RejectLeaveRequestC1Command(
    string? ActorIdpSubject,
    Guid RequestId,
    string? ReviewNote) : ICommand;

public sealed class RejectLeaveRequestC1CommandHandler(
    IIdentityAccountReadRepository accounts,
    IEmployeeReadRepository employees,
    ILeaveRequestRepository requests)
    : IAsyncCommandHandler<RejectLeaveRequestC1Command, LeaveRequestActionResult>
{
    public async Task<LeaveRequestActionResult> HandleAsync(
        RejectLeaveRequestC1Command command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);
        await LevLineManagerGuard
            .RequireLineManagerOfAsync(
                accounts,
                employees,
                requests,
                command.ActorIdpSubject,
                command.RequestId,
                cancellationToken)
            .ConfigureAwait(false);

        var request = await requests.FindByIdAsync(command.RequestId, cancellationToken).ConfigureAwait(false)!;
        if (request!.Status != LeaveRequestStatus.PendingC1)
        {
            throw new ConflictException(HrmErrorCodes.Conflict, "Đơn không còn chờ duyệt C1.");
        }

        var rejected = await requests
            .RejectC1Async(command.RequestId, command.ActorIdpSubject!, command.ReviewNote, cancellationToken)
            .ConfigureAwait(false);
        if (!rejected)
            throw new NotFoundException(HrmErrorCodes.NotFound, "Không từ chối C1 được đơn.");

        return new LeaveRequestActionResult(command.RequestId, LeaveRequestStatus.Rejected.ToString());
    }
}
