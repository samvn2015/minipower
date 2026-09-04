using Hrm.Application.Probation.Dtos;
using Hrm.Domain.Employees.Repositories;
using Hrm.Domain.Identity.Repositories;
using Hrm.Domain.Probation;
using Hrm.Domain.Shared.Constants;
using Jarvis.Application.Contracts.Queries;
using Jarvis.Domain.Shared.ExceptionHandling;
using Jarvis.Domain.Shared.Messaging;

namespace Hrm.Application.Probation.Queries;

/// <summary>Mốc TV của NV đăng nhập — PRB-FR-001 · FR-015 · SCR-004 (read-only EMP).</summary>
public sealed record GetMyProbationMilestonesQuery(string ActorIdpSubject) : IQuery;

public sealed class GetMyProbationMilestonesQueryHandler(
    IIdentityAccountReadRepository accounts,
    IEmployeeReadRepository employees)
    : IAsyncQueryHandler<GetMyProbationMilestonesQuery, ProbationMilestoneDto>
{
    public async Task<ProbationMilestoneDto> HandleAsync(
        GetMyProbationMilestonesQuery request,
        CancellationToken cancellationToken = default)
    {
        var actor = await accounts.FindByIdpSubjectAsync(request.ActorIdpSubject, cancellationToken)
            ?? throw new ForbiddenException(HrmErrorCodes.Forbidden, "Tài khoản không map.");
        PrbAccessGuard.RequireAuthenticated(actor);

        if (string.IsNullOrWhiteSpace(actor.EmployeeCode))
            throw new BadRequestException(HrmErrorCodes.BadRequest, "Tài khoản chưa gắn mã NV.");

        var emp = await employees.FindByEmployeeCodeAsync(actor.EmployeeCode, cancellationToken)
            ?? throw new NotFoundException(HrmErrorCodes.NotFound, "Không tìm thấy hồ sơ NV.");

        var onProbation = ProbationContractFacts.IsActiveProbationContract(emp.Contract);
        var (start, end, complete) = ProbationContractFacts.ReadMilestones(emp.Contract);

        return new ProbationMilestoneDto(
            emp.Id,
            emp.EmployeeCode,
            emp.FullName,
            emp.Contract?.ContractType,
            start,
            end,
            onProbation,
            complete,
            end is { } kt ? ProbationContractFacts.ComputeT15Date(kt) : null,
            end is { } kt2 ? ProbationContractFacts.ComputeT7Date(kt2) : null,
            Source: "EMP.Contract");
    }
}
