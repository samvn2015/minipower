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

public sealed record UpsertLifOffChecklistTickCommand(
    string ActorIdpSubject,
    Guid CaseId,
    string ItemCode,
    bool IsChecked) : ICommand;

public sealed record CloseLifOffboardingCommand(
    string ActorIdpSubject,
    Guid CaseId) : ICommand;

public sealed class UpsertLifOffChecklistTickCommandHandler(
    IIdentityAccountReadRepository accounts,
    ILifOffboardingRepository offboardings,
    ILifOffChecklistRepository checklist)
    : IAsyncCommandHandler<UpsertLifOffChecklistTickCommand, LifOffChecklistBoardDto>
{
    public async Task<LifOffChecklistBoardDto> HandleAsync(
        UpsertLifOffChecklistTickCommand request,
        CancellationToken cancellationToken = default)
    {
        var actor = await accounts.FindByIdpSubjectAsync(request.ActorIdpSubject, cancellationToken)
            ?? throw new ForbiddenException(HrmErrorCodes.Forbidden, "Tài khoản không map.");
        LifAccessGuard.RequireHrOrPgd(actor);

        var existing = await offboardings.FindByIdAsync(request.CaseId, cancellationToken)
            ?? throw new NotFoundException(HrmErrorCodes.NotFound, "Không tìm thấy case offboarding.");
        if (existing.Status == LifOffboardingStatus.Closed)
            throw new ConflictException(HrmErrorCodes.Conflict, "Case đã đóng.");

        var items = await checklist.ListActiveItemsAsync(cancellationToken);
        if (!items.Any(i => string.Equals(i.Code, request.ItemCode, StringComparison.OrdinalIgnoreCase)))
            throw new BadRequestException(HrmErrorCodes.BadRequest, "Mã checklist không thuộc master (LIF-FR-009).");

        await checklist.UpsertTickAsync(
            request.CaseId,
            request.ItemCode.Trim(),
            request.IsChecked,
            request.ActorIdpSubject,
            cancellationToken);

        return await LifOffChecklistBoardBuilder.BuildAsync(checklist, existing, cancellationToken);
    }
}

public sealed class CloseLifOffboardingCommandHandler(
    IIdentityAccountReadRepository accounts,
    ILifOffboardingRepository offboardings,
    ILifOffChecklistRepository checklist)
    : IAsyncCommandHandler<CloseLifOffboardingCommand, LifOffboardingDto>
{
    public async Task<LifOffboardingDto> HandleAsync(
        CloseLifOffboardingCommand request,
        CancellationToken cancellationToken = default)
    {
        var actor = await accounts.FindByIdpSubjectAsync(request.ActorIdpSubject, cancellationToken)
            ?? throw new ForbiddenException(HrmErrorCodes.Forbidden, "Tài khoản không map.");
        LifAccessGuard.RequireHrOrPgd(actor);

        var existing = await offboardings.FindByIdAsync(request.CaseId, cancellationToken)
            ?? throw new NotFoundException(HrmErrorCodes.NotFound, "Không tìm thấy case offboarding.");
        if (existing.Status == LifOffboardingStatus.Closed)
            throw new ConflictException(HrmErrorCodes.Conflict, "Case đã đóng.");

        if (!await checklist.AllMustCheckedAsync(request.CaseId, cancellationToken))
            throw new BadRequestException(
                HrmErrorCodes.BadRequest,
                "Không đóng off khi thiếu tick Must (LIF-FR-009).");

        var snap = await offboardings.CloseAsync(request.CaseId, request.ActorIdpSubject, cancellationToken);
        return LifOffboardingMapper.ToDto(snap);
    }
}

internal static class LifOffChecklistBoardBuilder
{
    public static async Task<LifOffChecklistBoardDto> BuildAsync(
        ILifOffChecklistRepository checklist,
        LifOffboardingSnapshot existing,
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

        var canClose = existing.Status != LifOffboardingStatus.Closed
            && await checklist.AllMustCheckedAsync(existing.Id, cancellationToken);

        return new LifOffChecklistBoardDto(existing.Id, existing.Status.ToString(), canClose, board);
    }
}

public sealed record ApplyLifOffboardingLocksCommand(
    string ActorIdpSubject,
    Guid CaseId,
    DateOnly? AsOfDate,
    string? EarlyCrReason) : ICommand;

public sealed record RunLifNPlus3LocksCommand(
    string ActorIdpSubject,
    DateOnly? AsOfDate) : ICommand;

public sealed class ApplyLifOffboardingLocksCommandHandler(
    IIdentityAccountReadRepository accounts,
    ILifOffboardingRepository offboardings)
    : IAsyncCommandHandler<ApplyLifOffboardingLocksCommand, LifOffboardingDto>
{
    public async Task<LifOffboardingDto> HandleAsync(
        ApplyLifOffboardingLocksCommand request,
        CancellationToken cancellationToken = default)
    {
        var actor = await accounts.FindByIdpSubjectAsync(request.ActorIdpSubject, cancellationToken)
            ?? throw new ForbiddenException(HrmErrorCodes.Forbidden, "Tài khoản không map.");
        LifAccessGuard.RequireItOrPgdForLocks(actor);

        var existing = await offboardings.FindByIdAsync(request.CaseId, cancellationToken)
            ?? throw new NotFoundException(HrmErrorCodes.NotFound, "Không tìm thấy case offboarding.");

        if (existing.Status != LifOffboardingStatus.ConfirmedN
            || existing.LastWorkingDayN is not { } n)
        {
            throw new BadRequestException(
                HrmErrorCodes.BadRequest,
                "Chỉ khóa khi N đã HR xác nhận (LIF-FR-005).");
        }

        if (existing.GitLockedAtUtc.HasValue && existing.CrmSpLockedAtUtc.HasValue)
            return LifOffboardingMapper.ToDto(existing);

        var asOf = request.AsOfDate ?? DateOnly.FromDateTime(DateTime.UtcNow);
        var cr = string.IsNullOrWhiteSpace(request.EarlyCrReason)
            ? null
            : request.EarlyCrReason.Trim();
        var early = cr is not null;

        if (!early && !LifOffboardingFacts.IsNPlus3Reached(n, asOf))
        {
            throw new BadRequestException(
                HrmErrorCodes.BadRequest,
                $"Cấm khóa trước N+3 ({LifOffboardingFacts.ComputeNPlus3(n):yyyy-MM-dd}) trừ CR an ninh (LIF-FR-007).");
        }

        var snap = await offboardings.ApplyAccessLocksAsync(
            new LifAccessLockApplyModel(
                request.CaseId,
                asOf,
                early,
                cr,
                request.ActorIdpSubject),
            cancellationToken);

        return LifOffboardingMapper.ToDto(snap);
    }
}

public sealed class RunLifNPlus3LocksCommandHandler(
    IIdentityAccountReadRepository accounts,
    ILifOffboardingRepository offboardings)
    : IAsyncCommandHandler<RunLifNPlus3LocksCommand, LifNPlus3LockRunResult>
{
    public async Task<LifNPlus3LockRunResult> HandleAsync(
        RunLifNPlus3LocksCommand request,
        CancellationToken cancellationToken = default)
    {
        var actor = await accounts.FindByIdpSubjectAsync(request.ActorIdpSubject, cancellationToken)
            ?? throw new ForbiddenException(HrmErrorCodes.Forbidden, "Tài khoản không map.");
        LifAccessGuard.RequireItOrPgdForLocks(actor);

        var asOf = request.AsOfDate ?? DateOnly.FromDateTime(DateTime.UtcNow);
        var all = await offboardings.ListAsync(cancellationToken);

        var locked = 0;
        var skippedNotDue = 0;
        var skippedAlready = 0;
        var skippedNoN = 0;

        foreach (var c in all)
        {
            if (c.Status != LifOffboardingStatus.ConfirmedN || c.LastWorkingDayN is not { } n)
            {
                skippedNoN++;
                continue;
            }

            if (c.GitLockedAtUtc.HasValue && c.CrmSpLockedAtUtc.HasValue)
            {
                skippedAlready++;
                continue;
            }

            // Job không CR — chỉ ≥ N+3 (FR-007).
            if (!LifOffboardingFacts.IsNPlus3Reached(n, asOf))
            {
                skippedNotDue++;
                continue;
            }

            await offboardings.ApplyAccessLocksAsync(
                new LifAccessLockApplyModel(
                    c.Id,
                    asOf,
                    IsEarlySecurityCr: false,
                    CrReason: null,
                    request.ActorIdpSubject),
                cancellationToken);
            locked++;
        }

        return new LifNPlus3LockRunResult(asOf, locked, skippedNotDue, skippedAlready, skippedNoN);
    }
}
