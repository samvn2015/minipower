using Hrm.Application.Common;
using Hrm.Application.Employees.Commands;
using Hrm.Domain.Employees;
using Hrm.Domain.Employees.Repositories;
using Hrm.Domain.Identity.Repositories;
using Hrm.Domain.Shared.Constants;
using Jarvis.Application.Contracts.Commands;
using Jarvis.Domain.Shared.ExceptionHandling;

namespace Hrm.Application.Employees.Commands;

public sealed class SubmitLineManagerChangeCommandHandler(
    IIdentityAccountReadRepository accounts,
    IOrgUnitReadRepository orgUnits,
    IEmployeeReadRepository employees,
    ILineManagerChangeRepository changes)
    : IAsyncCommandHandler<SubmitLineManagerChangeCommand, LineManagerChangeResult>
{
    public async Task<LineManagerChangeResult> HandleAsync(
        SubmitLineManagerChangeCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);
        IamAccessGuard.RequireAuthenticated(command.ActorIdpSubject);

        var actor = await accounts.FindByIdpSubjectAsync(command.ActorIdpSubject!, cancellationToken)
            .ConfigureAwait(false);
        if (actor is null || actor.Status != Domain.Identity.IdentityAccountStatus.Active)
            throw new ForbiddenException(HrmErrorCodes.Forbidden, "Tài khoản không hiệu lực.");

        if (!IamAccessGuard.IsHrOrIt(actor))
            throw new ForbiddenException(HrmErrorCodes.Forbidden, "Chỉ HR gửi đổi LM (EMP-SCR-005).");

        if (command.EmployeeId == command.ProposedLineManagerEmployeeId)
            throw new BadRequestException(HrmErrorCodes.BadRequest, "LM mới không thể là chính NV.");

        var employee = await employees.FindByIdAsync(command.EmployeeId, cancellationToken).ConfigureAwait(false)
            ?? throw new NotFoundException(HrmErrorCodes.NotFound, "NV không tồn tại.");

        if (employee.Status != EmployeeStatus.Active)
            throw new BadRequestException(HrmErrorCodes.BadRequest, "NV không hiệu lực.");

        var proposedLm = await employees.FindByIdAsync(command.ProposedLineManagerEmployeeId, cancellationToken)
            .ConfigureAwait(false)
            ?? throw new NotFoundException(HrmErrorCodes.NotFound, "LM mới không tồn tại.");

        if (proposedLm.Status != EmployeeStatus.Active)
            throw new BadRequestException(HrmErrorCodes.BadRequest, "LM mới không hiệu lực (EMP-FR-016).");

        if (!string.IsNullOrWhiteSpace(proposedLm.OrgUnitCode)
            && !await orgUnits.IsActiveAsync(proposedLm.OrgUnitCode, cancellationToken).ConfigureAwait(false))
        {
            throw new BadRequestException(HrmErrorCodes.BadRequest, "Org LM mới không hiệu lực (EMP-FR-016).");
        }

        var pending = await changes.FindPendingByEmployeeIdAsync(command.EmployeeId, cancellationToken)
            .ConfigureAwait(false);
        if (pending is not null)
            throw new ConflictException(HrmErrorCodes.Conflict, "Đã có đề xuất đổi LM đang chờ duyệt.");

        var requestId = await changes.CreateAsync(
            new LineManagerChangeCreateModel(
                command.EmployeeId,
                command.ProposedLineManagerEmployeeId,
                command.ActorIdpSubject!),
            cancellationToken).ConfigureAwait(false);

        return new LineManagerChangeResult(requestId, LineManagerChangeStatus.Pending.ToString());
    }
}

public sealed record ApproveLineManagerChangeCommand(string? ActorIdpSubject, Guid RequestId) : Jarvis.Domain.Shared.Messaging.ICommand;

public sealed class ApproveLineManagerChangeCommandHandler(
    IIdentityAccountReadRepository accounts,
    ILineManagerChangeRepository changes)
    : IAsyncCommandHandler<ApproveLineManagerChangeCommand, LineManagerChangeResult>
{
    public async Task<LineManagerChangeResult> HandleAsync(
        ApproveLineManagerChangeCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);
        IamAccessGuard.RequireAuthenticated(command.ActorIdpSubject);

        var actor = await accounts.FindByIdpSubjectAsync(command.ActorIdpSubject!, cancellationToken)
            .ConfigureAwait(false);
        IamAccessGuard.RequireHrOrPgd(actor);

        var request = await changes.FindByIdAsync(command.RequestId, cancellationToken).ConfigureAwait(false)
            ?? throw new NotFoundException(HrmErrorCodes.NotFound, "Đề xuất không tồn tại.");

        if (request.Status != LineManagerChangeStatus.Pending)
            throw new ConflictException(HrmErrorCodes.Conflict, "Đề xuất không còn chờ duyệt.");

        var approved = await changes.ApproveAsync(
            command.RequestId,
            request.EmployeeId,
            request.ProposedLineManagerEmployeeId,
            command.ActorIdpSubject!,
            cancellationToken).ConfigureAwait(false);

        if (!approved)
            throw new NotFoundException(HrmErrorCodes.NotFound, "Không duyệt được đề xuất.");

        return new LineManagerChangeResult(command.RequestId, LineManagerChangeStatus.Approved.ToString());
    }
}

public sealed record RejectLineManagerChangeCommand(
    string? ActorIdpSubject,
    Guid RequestId,
    string? ReviewNote) : Jarvis.Domain.Shared.Messaging.ICommand;

public sealed class RejectLineManagerChangeCommandHandler(
    IIdentityAccountReadRepository accounts,
    ILineManagerChangeRepository changes)
    : IAsyncCommandHandler<RejectLineManagerChangeCommand, LineManagerChangeResult>
{
    public async Task<LineManagerChangeResult> HandleAsync(
        RejectLineManagerChangeCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);
        IamAccessGuard.RequireAuthenticated(command.ActorIdpSubject);

        var actor = await accounts.FindByIdpSubjectAsync(command.ActorIdpSubject!, cancellationToken)
            .ConfigureAwait(false);
        IamAccessGuard.RequireHrOrPgd(actor);

        var request = await changes.FindByIdAsync(command.RequestId, cancellationToken).ConfigureAwait(false)
            ?? throw new NotFoundException(HrmErrorCodes.NotFound, "Đề xuất không tồn tại.");

        if (request.Status != LineManagerChangeStatus.Pending)
            throw new ConflictException(HrmErrorCodes.Conflict, "Đề xuất không còn chờ duyệt.");

        var rejected = await changes.RejectAsync(
            command.RequestId,
            command.ActorIdpSubject!,
            command.ReviewNote,
            cancellationToken).ConfigureAwait(false);

        if (!rejected)
            throw new NotFoundException(HrmErrorCodes.NotFound, "Không từ chối được đề xuất.");

        return new LineManagerChangeResult(command.RequestId, LineManagerChangeStatus.Rejected.ToString());
    }
}
