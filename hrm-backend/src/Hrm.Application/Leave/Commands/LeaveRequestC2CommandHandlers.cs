using Hrm.Application.Common;
using Hrm.Application.Leave.Dtos;
using Hrm.Domain.Identity.Repositories;
using Hrm.Domain.Leave;
using Hrm.Domain.Leave.Repositories;
using Hrm.Domain.Shared.Constants;
using Jarvis.Application.Contracts.Commands;
using Jarvis.Domain.Shared.ExceptionHandling;
using Jarvis.Domain.Shared.Messaging;

namespace Hrm.Application.Leave.Commands;

public sealed record ApproveLeaveRequestC2Command(string? ActorIdpSubject, Guid RequestId) : ICommand;

public sealed class ApproveLeaveRequestC2CommandHandler(
    IIdentityAccountReadRepository accounts,
    ILeaveRequestRepository requests,
    ILeaveTypeReadRepository leaveTypes)
    : IAsyncCommandHandler<ApproveLeaveRequestC2Command, LeaveRequestActionResult>
{
    public async Task<LeaveRequestActionResult> HandleAsync(
        ApproveLeaveRequestC2Command command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);
        IamAccessGuard.RequireAuthenticated(command.ActorIdpSubject);

        var actor = await accounts.FindByIdpSubjectAsync(command.ActorIdpSubject!, cancellationToken)
            .ConfigureAwait(false);
        LevHrGuard.RequireHrForC2(actor);

        var request = await requests.FindByIdAsync(command.RequestId, cancellationToken).ConfigureAwait(false)
            ?? throw new NotFoundException(HrmErrorCodes.NotFound, "Đơn nghỉ phép không tồn tại.");

        if (request.Status != LeaveRequestStatus.PendingC2)
        {
            throw new ConflictException(HrmErrorCodes.Conflict, "Đơn phải qua C1 trước khi duyệt C2 (LEV-TC-011).");
        }

        var leaveType = await leaveTypes.FindByCodeAsync(request.LeaveTypeCode, cancellationToken)
            .ConfigureAwait(false)
            ?? throw new BadRequestException(HrmErrorCodes.BadRequest, "Loại phép không hợp lệ.");

        var approved = await requests
            .ApproveC2Async(
                command.RequestId,
                command.ActorIdpSubject!,
                leaveType.DeductsAnnualBalance,
                cancellationToken)
            .ConfigureAwait(false);

        if (!approved)
        {
            throw new BadRequestException(
                HrmErrorCodes.BadRequest,
                "Không duyệt C2 — quỹ phép không đủ hoặc đơn không hợp lệ (LEV-FR-004).");
        }

        return new LeaveRequestActionResult(command.RequestId, LeaveRequestStatus.Approved.ToString());
    }
}

public sealed record RejectLeaveRequestC2Command(
    string? ActorIdpSubject,
    Guid RequestId,
    string? ReviewNote) : ICommand;

public sealed class RejectLeaveRequestC2CommandHandler(
    IIdentityAccountReadRepository accounts,
    ILeaveRequestRepository requests)
    : IAsyncCommandHandler<RejectLeaveRequestC2Command, LeaveRequestActionResult>
{
    public async Task<LeaveRequestActionResult> HandleAsync(
        RejectLeaveRequestC2Command command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);
        IamAccessGuard.RequireAuthenticated(command.ActorIdpSubject);

        var actor = await accounts.FindByIdpSubjectAsync(command.ActorIdpSubject!, cancellationToken)
            .ConfigureAwait(false);
        LevHrGuard.RequireHrForC2(actor);

        var request = await requests.FindByIdAsync(command.RequestId, cancellationToken).ConfigureAwait(false)
            ?? throw new NotFoundException(HrmErrorCodes.NotFound, "Đơn nghỉ phép không tồn tại.");

        if (request.Status != LeaveRequestStatus.PendingC2)
        {
            throw new ConflictException(HrmErrorCodes.Conflict, "Đơn không còn chờ duyệt C2.");
        }

        var rejected = await requests
            .RejectC2Async(command.RequestId, command.ActorIdpSubject!, command.ReviewNote, cancellationToken)
            .ConfigureAwait(false);
        if (!rejected)
            throw new NotFoundException(HrmErrorCodes.NotFound, "Không từ chối C2 được đơn.");

        return new LeaveRequestActionResult(command.RequestId, LeaveRequestStatus.Rejected.ToString());
    }
}
