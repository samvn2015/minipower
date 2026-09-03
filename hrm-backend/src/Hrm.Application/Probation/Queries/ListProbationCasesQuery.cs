using Hrm.Application.Probation.Dtos;
using Hrm.Domain.Employees;
using Hrm.Domain.Employees.Repositories;
using Hrm.Domain.Identity.Repositories;
using Hrm.Domain.Probation;
using Hrm.Domain.Shared.Constants;
using Jarvis.Application.Contracts.Queries;
using Jarvis.Domain.Shared.ExceptionHandling;
using Jarvis.Domain.Shared.Messaging;

namespace Hrm.Application.Probation.Queries;

/// <summary>Hàng NV đang TV — PRB-FR-001 · FR-008 · SCR-001.</summary>
public sealed record ListProbationCasesQuery(string ActorIdpSubject) : IQuery;

public sealed class ListProbationCasesQueryHandler(
    IIdentityAccountReadRepository accounts,
    IEmployeeReadRepository employees)
    : IAsyncQueryHandler<ListProbationCasesQuery, IReadOnlyList<ProbationCaseDto>>
{
    public async Task<IReadOnlyList<ProbationCaseDto>> HandleAsync(
        ListProbationCasesQuery request,
        CancellationToken cancellationToken = default)
    {
        var actor = await accounts.FindByIdpSubjectAsync(request.ActorIdpSubject, cancellationToken)
            ?? throw new ForbiddenException(HrmErrorCodes.Forbidden, "Tài khoản không map.");
        PrbAccessGuard.RequireHrOrPgd(actor);

        var all = await employees.ListAsync(cancellationToken);
        return all
            .Where(e => e.Status == EmployeeStatus.Active)
            .Where(e => ProbationContractFacts.IsActiveProbationContract(e.Contract))
            .Select(MapCase)
            .OrderBy(c => c.ProbationEndDate ?? DateOnly.MaxValue)
            .ThenBy(c => c.EmployeeCode)
            .ToList();
    }

    internal static ProbationCaseDto MapCase(EmployeeSnapshot e)
    {
        var (start, end, complete) = ProbationContractFacts.ReadMilestones(e.Contract);
        // start always set when IsActiveProbationContract (StartDate required on HĐ)
        return new ProbationCaseDto(
            e.Id,
            e.EmployeeCode,
            e.FullName,
            e.Contract!.ContractType,
            start!.Value,
            end,
            complete,
            end is { } kt ? ProbationContractFacts.ComputeT15Date(kt) : null,
            end is { } kt2 ? ProbationContractFacts.ComputeT7Date(kt2) : null);
    }
}
