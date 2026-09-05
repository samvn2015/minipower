using Hrm.Application.Probation.Dtos;
using Hrm.Domain.Identity.Repositories;
using Hrm.Domain.Probation;
using Hrm.Domain.Probation.Repositories;
using Hrm.Domain.Shared.Constants;
using Jarvis.Application.Contracts.Queries;
using Jarvis.Domain.Shared.ExceptionHandling;
using Jarvis.Domain.Shared.Messaging;

namespace Hrm.Application.Probation.Queries;

public sealed record ListProbationRemindersQuery(
    string ActorIdpSubject,
    string? Kind) : IQuery;

public sealed class ListProbationRemindersQueryHandler(
    IIdentityAccountReadRepository accounts,
    IProbationReminderRepository reminders)
    : IAsyncQueryHandler<ListProbationRemindersQuery, IReadOnlyList<ProbationReminderDto>>
{
    public async Task<IReadOnlyList<ProbationReminderDto>> HandleAsync(
        ListProbationRemindersQuery request,
        CancellationToken cancellationToken = default)
    {
        var actor = await accounts.FindByIdpSubjectAsync(request.ActorIdpSubject, cancellationToken)
            ?? throw new ForbiddenException(HrmErrorCodes.Forbidden, "Tài khoản không map.");
        PrbAccessGuard.RequireHrOrPgd(actor);

        ProbationReminderKind? kind = null;
        if (!string.IsNullOrWhiteSpace(request.Kind))
        {
            if (!Enum.TryParse<ProbationReminderKind>(request.Kind, ignoreCase: true, out var parsed))
                throw new BadRequestException(HrmErrorCodes.BadRequest, "Kind phải là T15 hoặc T7.");
            kind = parsed;
        }

        var rows = await reminders.ListAsync(kind, cancellationToken);
        return rows.Select(r => new ProbationReminderDto(
            r.Id,
            r.Kind.ToString(),
            r.EmployeeId,
            r.EmployeeCode,
            r.ProbationEndDate,
            r.DueDate,
            r.AsOfDate,
            r.AssigneeEmployeeId,
            r.AssigneeEmployeeCode,
            r.InAppMessage,
            r.EmailTo,
            r.Channel,
            r.CreatedAtUtc)).ToList();
    }
}
