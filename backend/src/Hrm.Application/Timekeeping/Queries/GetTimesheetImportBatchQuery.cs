using Hrm.Application.Common;
using Hrm.Application.Timekeeping.Dtos;
using Hrm.Domain.Identity.Repositories;
using Hrm.Domain.Shared.Constants;
using Hrm.Domain.Timekeeping.Repositories;
using Jarvis.Application.Contracts.Queries;
using Jarvis.Domain.Shared.ExceptionHandling;
using Jarvis.Domain.Shared.Messaging;

namespace Hrm.Application.Timekeeping.Queries;

public sealed record GetTimesheetImportBatchQuery(string? ActorIdpSubject, Guid BatchId) : IQuery;

public sealed class GetTimesheetImportBatchQueryHandler(
    IIdentityAccountReadRepository accounts,
    ITimesheetImportRepository imports)
    : IAsyncQueryHandler<GetTimesheetImportBatchQuery, TimesheetImportBatchDto>
{
    public async Task<TimesheetImportBatchDto> HandleAsync(
        GetTimesheetImportBatchQuery query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);
        IamAccessGuard.RequireAuthenticated(query.ActorIdpSubject);
        var actor = await accounts.FindByIdpSubjectAsync(query.ActorIdpSubject!, cancellationToken)
            .ConfigureAwait(false);
        TimHrGuard.RequireHrForImport(actor);

        var batch = await imports.FindBatchByIdAsync(query.BatchId, cancellationToken).ConfigureAwait(false)
            ?? throw new NotFoundException(HrmErrorCodes.NotFound, "Batch import không tồn tại.");

        return Commands.TimImportDtoMapper.Map(batch);
    }
}
