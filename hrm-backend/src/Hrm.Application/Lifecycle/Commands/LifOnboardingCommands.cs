using Hrm.Application.Lifecycle.Dtos;
using Hrm.Domain.Employees.Repositories;
using Hrm.Domain.Identity.Repositories;
using Hrm.Domain.Lifecycle;
using Hrm.Domain.Lifecycle.Repositories;
using Hrm.Domain.Shared.Constants;
using Jarvis.Application.Contracts.Commands;
using Jarvis.Application.Contracts.Queries;
using Jarvis.Domain.Shared.ExceptionHandling;
using Jarvis.Domain.Shared.Messaging;

namespace Hrm.Application.Lifecycle.Commands;

public sealed record CreateLifOnboardingCommand(
    string ActorIdpSubject,
    Guid EmployeeId,
    string? Note) : ICommand;

public sealed record UpsertLifOnChecklistTickCommand(
    string ActorIdpSubject,
    Guid CaseId,
    string ItemCode,
    bool IsChecked) : ICommand;

public sealed record MarkLifOnboardingProvisionedCommand(
    string ActorIdpSubject,
    Guid CaseId,
    string SystemCode,
    bool DeferGitToNPlus3) : ICommand;

public sealed record CloseLifOnboardingCommand(
    string ActorIdpSubject,
    Guid CaseId) : ICommand;

public sealed class CreateLifOnboardingCommandHandler(
    IIdentityAccountReadRepository accounts,
    IEmployeeReadRepository employees,
    ILifOnboardingRepository onboardings)
    : IAsyncCommandHandler<CreateLifOnboardingCommand, LifOnboardingDto>
{
    public async Task<LifOnboardingDto> HandleAsync(
        CreateLifOnboardingCommand request,
        CancellationToken cancellationToken = default)
    {
        var actor = await accounts.FindByIdpSubjectAsync(request.ActorIdpSubject, cancellationToken)
            ?? throw new ForbiddenException(HrmErrorCodes.Forbidden, "Tài khoản không map.");
        LifAccessGuard.RequireHrOrPgd(actor);

        var emp = await employees.FindByIdAsync(request.EmployeeId, cancellationToken)
            ?? throw new NotFoundException(HrmErrorCodes.NotFound, "Không tìm thấy NV.");

        var snap = await onboardings.CreateAsync(
            new LifOnboardingCreateModel(emp.Id, emp.EmployeeCode, request.ActorIdpSubject, request.Note),
            cancellationToken);
        return LifOnboardingMapper.ToDto(snap);
    }
}

public sealed class UpsertLifOnChecklistTickCommandHandler(
    IIdentityAccountReadRepository accounts,
    ILifOnboardingRepository onboardings,
    ILifOnChecklistRepository checklist)
    : IAsyncCommandHandler<UpsertLifOnChecklistTickCommand, LifOffChecklistBoardDto>
{
    public async Task<LifOffChecklistBoardDto> HandleAsync(
        UpsertLifOnChecklistTickCommand request,
        CancellationToken cancellationToken = default)
    {
        var actor = await accounts.FindByIdpSubjectAsync(request.ActorIdpSubject, cancellationToken)
            ?? throw new ForbiddenException(HrmErrorCodes.Forbidden, "Tài khoản không map.");
        LifAccessGuard.RequireHrOrPgd(actor);

        var existing = await onboardings.FindByIdAsync(request.CaseId, cancellationToken)
            ?? throw new NotFoundException(HrmErrorCodes.NotFound, "Không tìm thấy case onboarding.");
        if (existing.Status == LifOnboardingStatus.Closed)
            throw new ConflictException(HrmErrorCodes.Conflict, "Case đã đóng.");

        var items = await checklist.ListActiveItemsAsync(cancellationToken);
        if (!items.Any(i => string.Equals(i.Code, request.ItemCode, StringComparison.OrdinalIgnoreCase)))
            throw new BadRequestException(HrmErrorCodes.BadRequest, "Mã checklist không thuộc master (LIF-FR-001).");

        await checklist.UpsertTickAsync(
            request.CaseId,
            request.ItemCode.Trim(),
            request.IsChecked,
            request.ActorIdpSubject,
            cancellationToken);

        return await LifOnChecklistBoardBuilder.BuildAsync(checklist, existing, cancellationToken);
    }
}

public sealed class MarkLifOnboardingProvisionedCommandHandler(
    IIdentityAccountReadRepository accounts,
    ILifOnboardingRepository onboardings)
    : IAsyncCommandHandler<MarkLifOnboardingProvisionedCommand, LifOnboardingDto>
{
    public async Task<LifOnboardingDto> HandleAsync(
        MarkLifOnboardingProvisionedCommand request,
        CancellationToken cancellationToken = default)
    {
        var actor = await accounts.FindByIdpSubjectAsync(request.ActorIdpSubject, cancellationToken)
            ?? throw new ForbiddenException(HrmErrorCodes.Forbidden, "Tài khoản không map.");
        // IT hoặc HR/PGD đánh dấu đã cấp (UI HR ticket → IT).
        LifAccessGuard.RequireHrItOrPgd(actor);

        if (request.DeferGitToNPlus3)
        {
            throw new BadRequestException(
                HrmErrorCodes.BadRequest,
                "Cấm trì hoãn cấp Git đến N+3 — phải cấp lúc on (LIF-FR-002).");
        }

        if (!LifProvisionSystems.IsKnown(request.SystemCode))
            throw new BadRequestException(HrmErrorCodes.BadRequest, "Hệ thống cấp không hợp lệ.");

        var existing = await onboardings.FindByIdAsync(request.CaseId, cancellationToken)
            ?? throw new NotFoundException(HrmErrorCodes.NotFound, "Không tìm thấy case onboarding.");
        if (existing.Status == LifOnboardingStatus.Closed)
            throw new ConflictException(HrmErrorCodes.Conflict, "Case đã đóng.");

        var snap = await onboardings.MarkProvisionedAsync(
            request.CaseId,
            request.SystemCode.Trim(),
            request.ActorIdpSubject,
            cancellationToken);
        return LifOnboardingMapper.ToDto(snap);
    }
}

public sealed class CloseLifOnboardingCommandHandler(
    IIdentityAccountReadRepository accounts,
    ILifOnboardingRepository onboardings,
    ILifOnChecklistRepository checklist)
    : IAsyncCommandHandler<CloseLifOnboardingCommand, LifOnboardingDto>
{
    public async Task<LifOnboardingDto> HandleAsync(
        CloseLifOnboardingCommand request,
        CancellationToken cancellationToken = default)
    {
        var actor = await accounts.FindByIdpSubjectAsync(request.ActorIdpSubject, cancellationToken)
            ?? throw new ForbiddenException(HrmErrorCodes.Forbidden, "Tài khoản không map.");
        LifAccessGuard.RequireHrOrPgd(actor);

        var existing = await onboardings.FindByIdAsync(request.CaseId, cancellationToken)
            ?? throw new NotFoundException(HrmErrorCodes.NotFound, "Không tìm thấy case onboarding.");
        if (existing.Status == LifOnboardingStatus.Closed)
            throw new ConflictException(HrmErrorCodes.Conflict, "Case đã đóng.");

        if (!await checklist.AllMustCheckedAsync(request.CaseId, cancellationToken))
            throw new BadRequestException(
                HrmErrorCodes.BadRequest,
                "Không đóng on nếu thiếu tick Must (LIF-FR-001).");

        if (!existing.EmailCtyProvisioned || !existing.GitProvisioned
            || !existing.CrmSpProvisioned || !existing.ChatProvisioned)
        {
            throw new BadRequestException(
                HrmErrorCodes.BadRequest,
                "Không đóng on nếu chưa cấp đủ Email/Git/CRM SP/chat (LIF-FR-002).");
        }

        var snap = await onboardings.CloseAsync(request.CaseId, request.ActorIdpSubject, cancellationToken);
        return LifOnboardingMapper.ToDto(snap);
    }
}

internal static class LifOnChecklistBoardBuilder
{
    public static async Task<LifOffChecklistBoardDto> BuildAsync(
        ILifOnChecklistRepository checklist,
        LifOnboardingSnapshot existing,
        CancellationToken cancellationToken)
    {
        var master = await checklist.ListActiveItemsAsync(cancellationToken);
        var ticks = await checklist.ListTicksAsync(existing.Id, cancellationToken);
        var tickMap = ticks.ToDictionary(t => t.ItemCode, StringComparer.OrdinalIgnoreCase);
        var board = master.Select(m =>
        {
            tickMap.TryGetValue(m.Code, out var t);
            return new LifOffChecklistItemDto(
                m.Code,
                m.Name,
                m.IsMust,
                m.SortOrder,
                t?.IsChecked ?? false,
                t?.CheckedByIdpSubject,
                t?.CheckedAtUtc);
        }).ToList();

        var mustOk = await checklist.AllMustCheckedAsync(existing.Id, cancellationToken);
        var provisionOk = existing.EmailCtyProvisioned && existing.GitProvisioned
            && existing.CrmSpProvisioned && existing.ChatProvisioned;
        var canClose = existing.Status != LifOnboardingStatus.Closed && mustOk && provisionOk;

        return new LifOffChecklistBoardDto(existing.Id, existing.Status.ToString(), canClose, board);
    }
}
