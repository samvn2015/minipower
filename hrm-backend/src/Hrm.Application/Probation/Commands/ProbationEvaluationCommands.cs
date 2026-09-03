using System.Text.Json;
using Hrm.Application.Probation.Dtos;
using Hrm.Domain.Employees.Repositories;
using Hrm.Domain.Identity;
using Hrm.Domain.Identity.Constants;
using Hrm.Domain.Identity.Repositories;
using Hrm.Domain.Probation;
using Hrm.Domain.Probation.Repositories;
using Hrm.Domain.Shared.Constants;
using Jarvis.Application.Contracts.Commands;
using Jarvis.Domain.Shared.ExceptionHandling;
using Jarvis.Domain.Shared.Messaging;

namespace Hrm.Application.Probation.Commands;

public sealed record ProposeProbationEvaluationCommand(
    string ActorIdpSubject,
    Guid EmployeeId,
    string OutcomeCode,
    string? Note,
    IReadOnlyList<ProbationCriterionScoreInput>? Scores) : ICommand;

public sealed record DecideProbationEvaluationCommand(
    string ActorIdpSubject,
    Guid EmployeeId,
    string OutcomeCode,
    string? Note,
    string? ExtendDurationCode,
    IReadOnlyList<ProbationCriterionScoreInput>? Scores) : ICommand;

public sealed record ProbationCriterionScoreInput(string CriterionCode, string? Comment);

public sealed class ProposeProbationEvaluationCommandHandler(
    IIdentityAccountReadRepository accounts,
    IEmployeeReadRepository employees,
    IProbationMasterReadRepository masters,
    IProbationEvaluationRepository evaluations)
    : IAsyncCommandHandler<ProposeProbationEvaluationCommand, ProbationEvaluationDto>
{
    public async Task<ProbationEvaluationDto> HandleAsync(
        ProposeProbationEvaluationCommand request,
        CancellationToken cancellationToken = default)
    {
        var actor = await accounts.FindByIdpSubjectAsync(request.ActorIdpSubject, cancellationToken)
            ?? throw new ForbiddenException(HrmErrorCodes.Forbidden, "Tài khoản không map.");
        PrbAccessGuard.RequireAuthenticated(actor);

        var isLm = actor.RoleCodes.Any(r =>
            string.Equals(r, IamRoleCodes.Lm, StringComparison.OrdinalIgnoreCase));
        var isHr = actor.RoleCodes.Any(r =>
            string.Equals(r, IamRoleCodes.Hr, StringComparison.OrdinalIgnoreCase)
            || string.Equals(r, IamRoleCodes.Pgd, StringComparison.OrdinalIgnoreCase));
        if (!isLm && !isHr)
            throw new ForbiddenException(HrmErrorCodes.Forbidden, "Chỉ LM/HR lưu đề xuất (PRB-FR-009).");

        var emp = await LoadActiveProbationAsync(employees, request.EmployeeId, cancellationToken);
        if (isLm && !isHr)
        {
            if (string.IsNullOrWhiteSpace(actor.EmployeeCode))
                throw new ForbiddenException(HrmErrorCodes.Forbidden, "LM chưa gắn mã NV.");
            var lm = await employees.FindByEmployeeCodeAsync(actor.EmployeeCode, cancellationToken)
                ?? throw new ForbiddenException(HrmErrorCodes.Forbidden, "Không tìm thấy hồ sơ LM.");
            if (emp.LineManagerEmployeeId != lm.Id)
                throw new ForbiddenException(HrmErrorCodes.Forbidden, "LM chỉ đề xuất cấp dưới trực tiếp.");
        }

        await ValidateOutcomeAndScoresAsync(masters, request.OutcomeCode, request.Scores, cancellationToken);

        var (_, end, complete) = ProbationContractFacts.ReadMilestones(emp.Contract);
        if (!complete || end is null)
            throw new BadRequestException(HrmErrorCodes.BadRequest, "HĐ thiếu KT_TV — sửa trên EMP (PRB-FR-001).");

        var json = SerializeScores(request.Scores);
        var snap = await evaluations.UpsertProposeAsync(
            emp.Id,
            emp.EmployeeCode,
            end.Value,
            request.OutcomeCode.Trim().ToUpperInvariant(),
            request.ActorIdpSubject,
            request.Note,
            json,
            cancellationToken);

        return ProbationEvaluationMapper.ToDto(snap);
    }

    internal static async Task<EmployeeSnapshot> LoadActiveProbationAsync(
        IEmployeeReadRepository employees,
        Guid employeeId,
        CancellationToken cancellationToken)
    {
        var emp = await employees.FindByIdAsync(employeeId, cancellationToken)
            ?? throw new NotFoundException(HrmErrorCodes.NotFound, "Không tìm thấy NV.");
        if (!ProbationContractFacts.IsActiveProbationContract(emp.Contract))
            throw new BadRequestException(HrmErrorCodes.BadRequest, "NV không còn HĐ thử việc.");
        return emp;
    }

    internal static async Task ValidateOutcomeAndScoresAsync(
        IProbationMasterReadRepository masters,
        string outcomeCode,
        IReadOnlyList<ProbationCriterionScoreInput>? scores,
        CancellationToken cancellationToken)
    {
        if (!await masters.OutcomeExistsAsync(outcomeCode, cancellationToken))
            throw new BadRequestException(
                HrmErrorCodes.BadRequest,
                "Mã kết quả không thuộc master (PRB-FR-004).");

        if (scores is null)
            return;

        foreach (var s in scores)
        {
            if (!await masters.CriterionExistsAsync(s.CriterionCode, cancellationToken))
                throw new BadRequestException(
                    HrmErrorCodes.BadRequest,
                    $"Tiêu chí '{s.CriterionCode}' không thuộc master (PRB-FR-012).");
        }
    }

    internal static string? SerializeScores(IReadOnlyList<ProbationCriterionScoreInput>? scores) =>
        scores is null || scores.Count == 0
            ? null
            : JsonSerializer.Serialize(scores.Select(s => new { criterionCode = s.CriterionCode, comment = s.Comment }));
}

public sealed class DecideProbationEvaluationCommandHandler(
    IIdentityAccountReadRepository accounts,
    IEmployeeReadRepository employees,
    IProbationMasterReadRepository masters,
    IProbationEvaluationRepository evaluations)
    : IAsyncCommandHandler<DecideProbationEvaluationCommand, ProbationEvaluationDto>
{
    public async Task<ProbationEvaluationDto> HandleAsync(
        DecideProbationEvaluationCommand request,
        CancellationToken cancellationToken = default)
    {
        var actor = await accounts.FindByIdpSubjectAsync(request.ActorIdpSubject, cancellationToken)
            ?? throw new ForbiddenException(HrmErrorCodes.Forbidden, "Tài khoản không map.");
        PrbAccessGuard.RequireHrOrPgd(actor); // FR-009 · FR-017 audit = HR

        var emp = await ProposeProbationEvaluationCommandHandler.LoadActiveProbationAsync(
            employees, request.EmployeeId, cancellationToken);

        await ProposeProbationEvaluationCommandHandler.ValidateOutcomeAndScoresAsync(
            masters, request.OutcomeCode, request.Scores, cancellationToken);

        string? extendCode = null;
        if (string.Equals(request.OutcomeCode, ProbationOutcomeCodes.Extend, StringComparison.OrdinalIgnoreCase))
        {
            if (string.IsNullOrWhiteSpace(request.ExtendDurationCode))
                throw new BadRequestException(
                    HrmErrorCodes.BadRequest,
                    "Gia hạn cần mã thời lượng master (PRB-FR-006).");
            var dur = await masters.FindExtendDurationAsync(request.ExtendDurationCode, cancellationToken)
                ?? throw new BadRequestException(
                    HrmErrorCodes.BadRequest,
                    "Mã thời lượng gia hạn không thuộc master (PRB-FR-006).");
            extendCode = dur.Code;
        }
        else if (!string.IsNullOrWhiteSpace(request.ExtendDurationCode))
        {
            throw new BadRequestException(
                HrmErrorCodes.BadRequest,
                "Chỉ outcome EXTEND mới có ExtendDurationCode.");
        }

        var (_, end, complete) = ProbationContractFacts.ReadMilestones(emp.Contract);
        if (!complete || end is null)
            throw new BadRequestException(HrmErrorCodes.BadRequest, "HĐ thiếu KT_TV — sửa trên EMP (PRB-FR-001).");

        var json = ProposeProbationEvaluationCommandHandler.SerializeScores(request.Scores);
        var snap = await evaluations.DecideAsync(
            emp.Id,
            emp.EmployeeCode,
            end.Value,
            request.OutcomeCode.Trim().ToUpperInvariant(),
            request.ActorIdpSubject,
            request.Note,
            extendCode,
            json,
            cancellationToken);

        return ProbationEvaluationMapper.ToDto(snap);
    }
}

internal static class ProbationEvaluationMapper
{
    public static ProbationEvaluationDto ToDto(ProbationEvaluationSnapshot s) =>
        new(
            s.Id,
            s.EmployeeId,
            s.EmployeeCode,
            s.ProbationEndDate,
            s.Status.ToString(),
            s.ProposedOutcomeCode,
            s.ProposedByIdpSubject,
            s.ProposedAtUtc,
            s.ProposedNote,
            s.CriteriaPayloadJson,
            s.DecidedOutcomeCode,
            s.DecidedByIdpSubject,
            s.DecidedAtUtc,
            s.DecisionNote,
            s.ExtendDurationCode);
}
