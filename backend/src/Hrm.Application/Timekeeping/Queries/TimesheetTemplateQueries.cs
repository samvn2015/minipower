using Hrm.Application.Common;
using Hrm.Application.Timekeeping.Dtos;
using Hrm.Domain.Identity.Repositories;
using Hrm.Domain.Timekeeping.Repositories;
using Jarvis.Application.Contracts.Queries;
using Jarvis.Domain.Shared.Messaging;

namespace Hrm.Application.Timekeeping.Queries;

public sealed record GetActiveTimesheetTemplateQuery(string? ActorIdpSubject) : IQuery;

public sealed class GetActiveTimesheetTemplateQueryHandler(
    IIdentityAccountReadRepository accounts,
    ITimesheetTemplateRepository templates)
    : IAsyncQueryHandler<GetActiveTimesheetTemplateQuery, TimesheetTemplateDto?>
{
    public async Task<TimesheetTemplateDto?> HandleAsync(
        GetActiveTimesheetTemplateQuery query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);
        IamAccessGuard.RequireAuthenticated(query.ActorIdpSubject);
        var actor = await accounts.FindByIdpSubjectAsync(query.ActorIdpSubject!, cancellationToken)
            .ConfigureAwait(false);
        TimHrGuard.RequireHrOrItForTemplate(actor);

        var active = await templates.FindActiveAsync(cancellationToken).ConfigureAwait(false);
        return active is null ? null : TimDtoMapper.Map(active);
    }
}

public sealed record ListTimesheetTemplatesQuery(string? ActorIdpSubject) : IQuery;

public sealed class ListTimesheetTemplatesQueryHandler(
    IIdentityAccountReadRepository accounts,
    ITimesheetTemplateRepository templates)
    : IAsyncQueryHandler<ListTimesheetTemplatesQuery, IReadOnlyList<TimesheetTemplateDto>>
{
    public async Task<IReadOnlyList<TimesheetTemplateDto>> HandleAsync(
        ListTimesheetTemplatesQuery query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);
        IamAccessGuard.RequireAuthenticated(query.ActorIdpSubject);
        var actor = await accounts.FindByIdpSubjectAsync(query.ActorIdpSubject!, cancellationToken)
            .ConfigureAwait(false);
        TimHrGuard.RequireHrOrItForTemplate(actor);

        var items = await templates.ListAsync(cancellationToken).ConfigureAwait(false);
        return items.Select(TimDtoMapper.Map).ToList();
    }
}

internal static class TimDtoMapper
{
    public static TimesheetTemplateDto Map(TimesheetTemplateVersionSnapshot snapshot) =>
        new(
            snapshot.Id,
            snapshot.VersionCode,
            snapshot.Name,
            snapshot.Status.ToString(),
            snapshot.PublishedAtUtc,
            snapshot.PublishedByIdpSubject,
            snapshot.Columns.Select(c => new TimesheetTemplateColumnDto(
                c.ColumnKey,
                c.DisplayName,
                c.SortOrder,
                c.IsRequired,
                c.MapsTo)).ToList());
}
