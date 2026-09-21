using Hrm.Application.Leave.Dtos;
using Hrm.Domain.Employees;
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
    ILeaveRequestRepository requests,
    ILeaveNotificationOutbox notifications,
    ILevAuditLogRepository auditLogs,
    ILevAtomicScope scope)
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

        // M1 (pass 4): nghiệp vụ + outbox + audit trong MỘT transaction — audit fail thì rollback cả.
        return await scope.RunAsync(async ct =>
        {
            var approved = await requests
                .ApproveC1Async(command.RequestId, command.ActorIdpSubject!, ct)
                .ConfigureAwait(false);
            if (!approved)
                throw new NotFoundException(HrmErrorCodes.NotFound, "Không duyệt C1 được đơn.");

            await LeaveNotify.EmitAsync(
                    notifications,
                    request.Id,
                    request.EmployeeId,
                    LeaveNotificationEvents.C1Approved,
                    ct)
                .ConfigureAwait(false);

            await auditLogs.AppendAsync(
                    new EmpAuditLogEntry(
                        EmpAuditActions.LeaveRequestC1Approved,
                        request.EmployeeId,
                        request.Id,
                        command.ActorIdpSubject!,
                        null),
                    ct)
                .ConfigureAwait(false);

            return new LeaveRequestActionResult(command.RequestId, LeaveRequestStatus.PendingC2.ToString());
        }, cancellationToken).ConfigureAwait(false);
    }
}

public sealed record RejectLeaveRequestC1Command(
    string? ActorIdpSubject,
    Guid RequestId,
    string? ReviewNote) : ICommand;

public sealed class RejectLeaveRequestC1CommandHandler(
    IIdentityAccountReadRepository accounts,
    IEmployeeReadRepository employees,
    ILeaveRequestRepository requests,
    ILevAuditLogRepository auditLogs,
    ILevAtomicScope scope)
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

        // M1 (pass 4): nghiệp vụ + outbox + audit trong MỘT transaction — audit fail thì rollback cả.
        return await scope.RunAsync(async ct =>
        {
            var rejected = await requests
                .RejectC1Async(command.RequestId, command.ActorIdpSubject!, command.ReviewNote, cancellationToken)
                .ConfigureAwait(false);
            if (!rejected)
                throw new NotFoundException(HrmErrorCodes.NotFound, "Không từ chối C1 được đơn.");

            await auditLogs.AppendAsync(
                    new EmpAuditLogEntry(
                        EmpAuditActions.LeaveRequestC1Rejected,
                        request!.EmployeeId,
                        request.Id,
                        command.ActorIdpSubject!,
                        command.ReviewNote),
                    cancellationToken)
                .ConfigureAwait(false);

            return new LeaveRequestActionResult(command.RequestId, LeaveRequestStatus.Rejected.ToString());
        }, cancellationToken).ConfigureAwait(false);
    }
}
