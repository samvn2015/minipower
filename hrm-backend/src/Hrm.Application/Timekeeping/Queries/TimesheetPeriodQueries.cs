using Hrm.Application.Common;
using Hrm.Application.Timekeeping.Commands;
using Hrm.Application.Timekeeping.Dtos;
using Hrm.Domain.Identity.Repositories;
using Hrm.Domain.Shared.Constants;
using Hrm.Domain.Timekeeping.Repositories;
using Jarvis.Application.Contracts.Queries;
using Jarvis.Domain.Shared.ExceptionHandling;
using Jarvis.Domain.Shared.Messaging;

namespace Hrm.Application.Timekeeping.Queries;

public sealed record ListTimesheetPeriodsQuery(string? ActorIdpSubject) : IQuery;

public sealed class ListTimesheetPeriodsQueryHandler(
    IIdentityAccountReadRepository accounts,
    ITimesheetImportRepository imports)
    : IAsyncQueryHandler<ListTimesheetPeriodsQuery, IReadOnlyList<TimesheetPeriodDto>>
{
    public async Task<IReadOnlyList<TimesheetPeriodDto>> HandleAsync(
        ListTimesheetPeriodsQuery query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);
        IamAccessGuard.RequireAuthenticated(query.ActorIdpSubject);
        var actor = await accounts.FindByIdpSubjectAsync(query.ActorIdpSubject!, cancellationToken)
            .ConfigureAwait(false);
        TimHrGuard.RequireHrForImport(actor);

        var periods = await imports.ListPeriodsAsync(cancellationToken).ConfigureAwait(false);
        return periods.Select(TimImportDtoMapper.MapPeriod).ToList();
    }
}

public sealed record GetTimesheetPeriodQuery(string? ActorIdpSubject, string PeriodYm) : IQuery;

public sealed class GetTimesheetPeriodQueryHandler(
    IIdentityAccountReadRepository accounts,
    ITimesheetImportRepository imports)
    : IAsyncQueryHandler<GetTimesheetPeriodQuery, TimesheetPeriodDto>
{
    public async Task<TimesheetPeriodDto> HandleAsync(
        GetTimesheetPeriodQuery query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);
        IamAccessGuard.RequireAuthenticated(query.ActorIdpSubject);
        var actor = await accounts.FindByIdpSubjectAsync(query.ActorIdpSubject!, cancellationToken)
            .ConfigureAwait(false);
        TimHrGuard.RequireHrForImport(actor);

        var period = await imports.FindPeriodByYmAsync(query.PeriodYm.Trim(), cancellationToken)
            .ConfigureAwait(false)
            ?? throw new NotFoundException(HrmErrorCodes.NotFound, $"Kỳ công {query.PeriodYm} không tồn tại.");

        return TimImportDtoMapper.MapPeriod(period);
    }
}
