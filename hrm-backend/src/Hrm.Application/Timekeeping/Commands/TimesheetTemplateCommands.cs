using Hrm.Application.Common;
using Hrm.Application.Timekeeping.Dtos;
using Hrm.Domain.Employees;
using Hrm.Domain.Employees.Repositories;
using Hrm.Domain.Identity.Repositories;
using Hrm.Domain.Shared.Constants;
using Hrm.Domain.Timekeeping;
using Hrm.Domain.Timekeeping.Repositories;
using Jarvis.Application.Contracts.Commands;
using Jarvis.Domain.Shared.ExceptionHandling;
using Jarvis.Domain.Shared.Messaging;

namespace Hrm.Application.Timekeeping.Commands;

public sealed record CreateTimesheetTemplateCommand(
    string? ActorIdpSubject,
    string VersionCode,
    string Name,
    IReadOnlyList<TimesheetTemplateColumnCreateModel> Columns) : ICommand;

public sealed class CreateTimesheetTemplateCommandHandler(
    IIdentityAccountReadRepository accounts,
    ITimesheetTemplateRepository templates)
    : IAsyncCommandHandler<CreateTimesheetTemplateCommand, TimesheetTemplateCreateResult>
{
    public async Task<TimesheetTemplateCreateResult> HandleAsync(
        CreateTimesheetTemplateCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);
        IamAccessGuard.RequireAuthenticated(command.ActorIdpSubject);
        var actor = await accounts.FindByIdpSubjectAsync(command.ActorIdpSubject!, cancellationToken)
            .ConfigureAwait(false);
        TimHrGuard.RequireHrOrItForTemplate(actor);

        if (string.IsNullOrWhiteSpace(command.VersionCode))
            throw new BadRequestException(HrmErrorCodes.BadRequest, "VersionCode bắt buộc.");

        if (string.IsNullOrWhiteSpace(command.Name))
            throw new BadRequestException(HrmErrorCodes.BadRequest, "Tên mẫu bắt buộc.");

        if (command.Columns.Count == 0)
            throw new BadRequestException(HrmErrorCodes.BadRequest, "Mẫu phải có ít nhất một cột master (TIM-FR-002).");

        var keys = command.Columns.Select(c => c.ColumnKey.Trim()).ToList();
        if (keys.Any(string.IsNullOrWhiteSpace) || keys.Distinct(StringComparer.OrdinalIgnoreCase).Count() != keys.Count)
            throw new BadRequestException(HrmErrorCodes.BadRequest, "ColumnKey trùng hoặc trống.");

        var code = command.VersionCode.Trim();
        if (await templates.ExistsByVersionCodeAsync(code, cancellationToken).ConfigureAwait(false))
            throw new ConflictException(HrmErrorCodes.Conflict, "VersionCode đã tồn tại.");

        var id = await templates.CreateDraftAsync(
            new TimesheetTemplateCreateModel(code, command.Name, command.Columns),
            cancellationToken).ConfigureAwait(false);

        return new TimesheetTemplateCreateResult(id, code, TimesheetTemplateStatus.Draft.ToString());
    }
}

public sealed record PublishTimesheetTemplateCommand(string? ActorIdpSubject, Guid TemplateId) : ICommand;

public sealed class PublishTimesheetTemplateCommandHandler(
    IIdentityAccountReadRepository accounts,
    ITimesheetTemplateRepository templates,
    IEmpAuditLogRepository auditLogs)
    : IAsyncCommandHandler<PublishTimesheetTemplateCommand, TimesheetTemplatePublishResult>
{
    public async Task<TimesheetTemplatePublishResult> HandleAsync(
        PublishTimesheetTemplateCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);
        IamAccessGuard.RequireAuthenticated(command.ActorIdpSubject);
        var actor = await accounts.FindByIdpSubjectAsync(command.ActorIdpSubject!, cancellationToken)
            .ConfigureAwait(false);
        TimHrGuard.RequireHrOrItForTemplate(actor);

        var before = await templates.FindByIdAsync(command.TemplateId, cancellationToken).ConfigureAwait(false)
            ?? throw new NotFoundException(HrmErrorCodes.NotFound, "Mẫu không tồn tại.");

        if (before.Status != TimesheetTemplateStatus.Draft)
            throw new ConflictException(HrmErrorCodes.Conflict, "Chỉ công bố mẫu Draft (TIM-FR-001).");

        var published = await templates
            .PublishAsync(command.TemplateId, command.ActorIdpSubject!, cancellationToken)
            .ConfigureAwait(false);
        if (!published)
            throw new BadRequestException(HrmErrorCodes.BadRequest, "Không công bố được mẫu (thiếu cột?).");

        var activeCount = await templates.CountActiveAsync(cancellationToken).ConfigureAwait(false);
        if (activeCount != 1)
            throw new InvalidOperationException("Invariant TIM-FR-015: phải đúng một mẫu Active.");

        await auditLogs.AppendAsync(
            new EmpAuditLogEntry(
                EmpAuditActions.TimesheetTemplatePublished,
                EmployeeId: null,
                command.TemplateId,
                command.ActorIdpSubject!,
                before.VersionCode),
            cancellationToken).ConfigureAwait(false);

        return new TimesheetTemplatePublishResult(
            command.TemplateId,
            before.VersionCode,
            TimesheetTemplateStatus.Active.ToString());
    }
}
