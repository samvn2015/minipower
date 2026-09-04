using Hrm.Application.Lifecycle.Dtos;
using Hrm.Domain.Employees.Repositories;
using Hrm.Domain.Identity.Repositories;
using Hrm.Domain.Lifecycle;
using Hrm.Domain.Lifecycle.Repositories;
using Hrm.Domain.Shared.Constants;
using Jarvis.Application.Contracts.Commands;
using Jarvis.Domain.Shared.ExceptionHandling;
using Jarvis.Domain.Shared.Messaging;

namespace Hrm.Application.Lifecycle.Commands;

public sealed record CreateLifOffboardingCommand(
    string ActorIdpSubject,
    Guid EmployeeId,
    DateOnly? ResignationSignedDate,
    string? Note) : ICommand;

public sealed record ConfirmLifOffboardingNCommand(
    string ActorIdpSubject,
    Guid CaseId,
    DateOnly LastWorkingDayN) : ICommand;

public sealed class CreateLifOffboardingCommandHandler(
    IIdentityAccountReadRepository accounts,
    IEmployeeReadRepository employees,
    ILifOffboardingRepository offboardings)
    : IAsyncCommandHandler<CreateLifOffboardingCommand, LifOffboardingDto>
{
    public const string SourceHrManual = "HR-MANUAL";

    public async Task<LifOffboardingDto> HandleAsync(
        CreateLifOffboardingCommand request,
        CancellationToken cancellationToken = default)
    {
        var actor = await accounts.FindByIdpSubjectAsync(request.ActorIdpSubject, cancellationToken)
            ?? throw new ForbiddenException(HrmErrorCodes.Forbidden, "Tài khoản không map.");
        LifAccessGuard.RequireHrOrPgd(actor);

        var emp = await employees.FindByIdAsync(request.EmployeeId, cancellationToken)
            ?? throw new NotFoundException(HrmErrorCodes.NotFound, "Không tìm thấy NV.");

        var snap = await offboardings.CreateAsync(
            new LifOffboardingCreateModel(
                emp.Id,
                emp.EmployeeCode,
                SourceHrManual,
                request.ActorIdpSubject,
                request.Note,
                request.ResignationSignedDate),
            cancellationToken);

        return LifOffboardingMapper.ToDto(snap);
    }
}

public sealed class ConfirmLifOffboardingNCommandHandler(
    IIdentityAccountReadRepository accounts,
    ILifOffboardingRepository offboardings)
    : IAsyncCommandHandler<ConfirmLifOffboardingNCommand, LifOffboardingDto>
{
    public async Task<LifOffboardingDto> HandleAsync(
        ConfirmLifOffboardingNCommand request,
        CancellationToken cancellationToken = default)
    {
        var actor = await accounts.FindByIdpSubjectAsync(request.ActorIdpSubject, cancellationToken)
            ?? throw new ForbiddenException(HrmErrorCodes.Forbidden, "Tài khoản không map.");
        LifAccessGuard.RequireHrOrPgd(actor); // FR-015: NV 403

        var existing = await offboardings.FindByIdAsync(request.CaseId, cancellationToken)
            ?? throw new NotFoundException(HrmErrorCodes.NotFound, "Không tìm thấy case offboarding.");

        if (existing.Status == LifOffboardingStatus.Closed)
            throw new ConflictException(HrmErrorCodes.Conflict, "Case đã đóng.");

        if (existing.ResignationSignedDate is { } rs
            && rs == request.LastWorkingDayN)
        {
            throw new BadRequestException(
                HrmErrorCodes.BadRequest,
                "N trùng ngày ký đơn đã lưu — N phải là ngày LV cuối, không phải ngày ký (LIF-FR-003).");
        }

        var snap = await offboardings.ConfirmNAsync(
            request.CaseId,
            request.LastWorkingDayN,
            request.ActorIdpSubject,
            cancellationToken);

        return LifOffboardingMapper.ToDto(snap);
    }
}
