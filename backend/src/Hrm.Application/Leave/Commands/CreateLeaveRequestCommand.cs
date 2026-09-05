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

public sealed record CreateLeaveRequestCommand(
    string? ActorIdpSubject,
    string LeaveTypeCode,
    DateOnly FromDate,
    DateOnly ToDate,
    LeaveDayPart DayPart,
    string Reason,
    Guid HandoverEmployeeId,
    bool IsEmergency,
    string? AttachmentFileName = null,
    bool AttachmentMatchesCompanyTemplate = false,
    DateOnly? SubmittedOn = null) : ICommand;

public sealed class CreateLeaveRequestCommandHandler(
    IIdentityAccountReadRepository accounts,
    IEmployeeReadRepository employees,
    ILeaveTypeReadRepository leaveTypes,
    ILeaveBalanceRepository balances,
    ILeaveRequestRepository requests,
    ILeaveNotificationOutbox notifications)
    : IAsyncCommandHandler<CreateLeaveRequestCommand, LeaveRequestCreateResult>
{
    public async Task<LeaveRequestCreateResult> HandleAsync(
        CreateLeaveRequestCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);
        var (_, employee) = await LevEmployeeGuard
            .ResolveActorEmployeeAsync(accounts, employees, command.ActorIdpSubject, cancellationToken)
            .ConfigureAwait(false);

        if (string.IsNullOrWhiteSpace(command.Reason))
            throw new BadRequestException(HrmErrorCodes.BadRequest, "Lý do bắt buộc (LEV-FR-001).");

        if (command.ToDate < command.FromDate)
            throw new BadRequestException(HrmErrorCodes.BadRequest, "Đến ngày phải >= Từ ngày.");

        if (command.HandoverEmployeeId == employee.Id)
            throw new BadRequestException(HrmErrorCodes.BadRequest, "Người bàn giao phải khác NV xin phép (LEV-BR-004).");

        var handover = await employees
            .FindByIdAsync(command.HandoverEmployeeId, cancellationToken)
            .ConfigureAwait(false)
            ?? throw new BadRequestException(HrmErrorCodes.BadRequest, "Người bàn giao không tồn tại.");

        if (handover.Status != EmployeeStatus.Active)
            throw new BadRequestException(HrmErrorCodes.BadRequest, "Người bàn giao phải đang active (LEV-BR-004).");

        var leaveType = await leaveTypes
            .FindByCodeAsync(command.LeaveTypeCode, cancellationToken)
            .ConfigureAwait(false)
            ?? throw new BadRequestException(HrmErrorCodes.BadRequest, "Loại phép không hợp lệ (LEV-BR-001).");

        if (leaveType.Status != LeaveTypeStatus.Active)
            throw new BadRequestException(HrmErrorCodes.BadRequest, "Loại phép không còn hiệu lực.");

        if (leaveType.RequiresCompanyTemplateFile)
        {
            if (string.IsNullOrWhiteSpace(command.AttachmentFileName)
                || !command.AttachmentMatchesCompanyTemplate)
            {
                throw new BadRequestException(
                    HrmErrorCodes.BadRequest,
                    "Ốm/BHXH bắt buộc file đúng mẫu Cty (LEV-FR-008).");
            }
        }

        var totalDays = LeaveDayCalculator.Calculate(command.FromDate, command.ToDate, command.DayPart);
        if (totalDays <= 0)
            throw new BadRequestException(HrmErrorCodes.BadRequest, "Số ngày đơn phải > 0.");

        var submittedOn = command.SubmittedOn ?? DateOnly.FromDateTime(DateTime.UtcNow);
        if (LeaveAdvanceNotice.IsLateWithoutEmergency(
                submittedOn,
                command.FromDate,
                command.ToDate,
                command.IsEmergency))
        {
            throw new BadRequestException(
                HrmErrorCodes.BadRequest,
                "Nộp trễ hạn 3 NLĐ — đánh dấu Nghỉ đột xuất (LEV-FR-006/007).");
        }

        if (leaveType.DeductsAnnualBalance)
        {
            var year = command.FromDate.Year;
            var balance = await balances
                .FindByEmployeeAndYearAsync(employee.Id, year, cancellationToken)
                .ConfigureAwait(false)
                ?? throw new BadRequestException(
                    HrmErrorCodes.BadRequest,
                    $"Chưa có quỹ phép năm {year}.");

            if (totalDays > balance.RemainingDays)
            {
                throw new BadRequestException(
                    HrmErrorCodes.BadRequest,
                    $"Vượt quỹ phép năm còn {balance.RemainingDays} ngày (LEV-FR-004).");
            }
        }

        if (await requests
                .HasOpenOverlapAsync(
                    employee.Id,
                    command.FromDate,
                    command.ToDate,
                    command.DayPart,
                    cancellationToken)
                .ConfigureAwait(false))
        {
            throw new BadRequestException(
                HrmErrorCodes.BadRequest,
                "Trùng ngày/buổi với đơn đang mở (LEV-FR-003).");
        }

        var id = await requests.CreateAsync(
            new LeaveRequestCreateModel(
                employee.Id,
                leaveType.Code,
                command.FromDate,
                command.ToDate,
                command.DayPart,
                totalDays,
                command.Reason.Trim(),
                command.HandoverEmployeeId,
                command.IsEmergency,
                string.IsNullOrWhiteSpace(command.AttachmentFileName)
                    ? null
                    : command.AttachmentFileName.Trim(),
                command.AttachmentMatchesCompanyTemplate),
            cancellationToken).ConfigureAwait(false);

        await LeaveNotify.EmitAsync(
                notifications,
                id,
                employee.Id,
                LeaveNotificationEvents.Submitted,
                cancellationToken)
            .ConfigureAwait(false);

        return new LeaveRequestCreateResult(id, LeaveRequestStatus.PendingC1.ToString(), totalDays);
    }
}
