using Hrm.Application.Probation.Commands;
using Hrm.Domain.Employees;
using Hrm.Domain.Employees.Repositories;
using Hrm.Domain.Identity;
using Hrm.Domain.Identity.Repositories;
using Hrm.Domain.Probation;
using Hrm.Domain.Probation.Repositories;
using Jarvis.Domain.Shared.ExceptionHandling;

namespace Hrm.Application.Tests.Probation;

public sealed class DecideProbationEvaluationCommandHandlerTests
{
    private static readonly Guid EmpId = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee");

    [Fact]
    public async Task Decide_Hr_Pass_RecordsAudit()
    {
        var evals = new FakeEvalRepo();
        var handler = new DecideProbationEvaluationCommandHandler(
            new FakeAccounts(["IAM-ROLE-HR"], "MNV-HR"),
            new FakeEmployees([TvEmp()]),
            new FakeMasters(),
            evals);

        var dto = await handler.HandleAsync(new DecideProbationEvaluationCommand(
            "local-dev", EmpId, ProbationOutcomeCodes.Pass, "OK", null, null));

        Assert.Equal("Decided", dto.Status);
        Assert.Equal(ProbationOutcomeCodes.Pass, dto.DecidedOutcomeCode);
        Assert.Equal("local-dev", dto.DecidedByIdpSubject);
        Assert.NotNull(dto.DecidedAtUtc);
    }

    [Fact]
    public async Task Decide_Lm_Forbidden()
    {
        var handler = new DecideProbationEvaluationCommandHandler(
            new FakeAccounts(["IAM-ROLE-LM", "IAM-ROLE-NV"], "MNV-LM"),
            new FakeEmployees([TvEmp()]),
            new FakeMasters(),
            new FakeEvalRepo());

        await Assert.ThrowsAsync<ForbiddenException>(() =>
            handler.HandleAsync(new DecideProbationEvaluationCommand(
                "local-lm", EmpId, ProbationOutcomeCodes.Pass, null, null, null)));
    }

    [Fact]
    public async Task Decide_InvalidOutcome_BadRequest()
    {
        var handler = new DecideProbationEvaluationCommandHandler(
            new FakeAccounts(["IAM-ROLE-HR"], "MNV-HR"),
            new FakeEmployees([TvEmp()]),
            new FakeMasters(),
            new FakeEvalRepo());

        await Assert.ThrowsAsync<BadRequestException>(() =>
            handler.HandleAsync(new DecideProbationEvaluationCommand(
                "local-dev", EmpId, "CONDITIONAL", null, null, null)));
    }

    [Fact]
    public async Task Decide_ExtendWithoutMaster_BadRequest()
    {
        var handler = new DecideProbationEvaluationCommandHandler(
            new FakeAccounts(["IAM-ROLE-HR"], "MNV-HR"),
            new FakeEmployees([TvEmp()]),
            new FakeMasters(),
            new FakeEvalRepo());

        await Assert.ThrowsAsync<BadRequestException>(() =>
            handler.HandleAsync(new DecideProbationEvaluationCommand(
                "local-dev", EmpId, ProbationOutcomeCodes.Extend, null, null, null)));
    }

    private static EmployeeSnapshot TvEmp() =>
        new(
            EmpId, "MNV-TV", "TV", null, "tv@company.local", null, null, null, null, null,
            new EmployeeContractSnapshot("PROBATION", new DateOnly(2026, 1, 1), new DateOnly(2026, 6, 30), true),
            null,
            EmployeeStatus.Active);

    private sealed class FakeAccounts(string[] roles, string? employeeCode) : IIdentityAccountReadRepository
    {
        public Task<IdentityAccountSnapshot?> FindByIdpSubjectAsync(
            string idpSubject,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IdentityAccountSnapshot?>(new IdentityAccountSnapshot(
                Guid.NewGuid(), idpSubject, idpSubject, null, employeeCode,
                IdentityAccountStatus.Active, roles));

        public Task<IdentityAccountSnapshot?> FindByEmployeeCodeAsync(
            string employeeCode,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IdentityAccountSnapshot?>(null);
    }

    private sealed class FakeEmployees(IReadOnlyList<EmployeeSnapshot> items) : IEmployeeReadRepository
    {
        public Task<IReadOnlyList<EmployeeSnapshot>> ListAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult(items);

        public Task<EmployeeSnapshot?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
            Task.FromResult(items.FirstOrDefault(e => e.Id == id));

        public Task<EmployeeSnapshot?> FindByEmployeeCodeAsync(
            string employeeCode,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(items.FirstOrDefault(e =>
                string.Equals(e.EmployeeCode, employeeCode, StringComparison.OrdinalIgnoreCase)));

        public Task<EmployeeSnapshot?> FindByEmailCtyAsync(
            string emailCty,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<EmployeeSnapshot?>(null);

        public Task<EmployeeUniqueField?> FindDuplicateAsync(
            string employeeCode,
            string? cccd,
            string? emailCty,
            string? taxId,
            Guid? excludeEmployeeId = null,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<EmployeeUniqueField?>(null);
    }

    private sealed class FakeMasters : IProbationMasterReadRepository
    {
        public Task<IReadOnlyList<ProbationOutcomeSnapshot>> ListOutcomesAsync(
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<ProbationOutcomeSnapshot>>([
                new(ProbationOutcomeCodes.Pass, "Đạt", 1),
                new(ProbationOutcomeCodes.Extend, "Gia hạn", 2),
                new(ProbationOutcomeCodes.Fail, "Không đạt", 3)
            ]);

        public Task<bool> OutcomeExistsAsync(string code, CancellationToken cancellationToken = default) =>
            Task.FromResult(ProbationOutcomeCodes.All.Contains(code));

        public Task<IReadOnlyList<ProbationCriterionSnapshot>> ListCriteriaAsync(
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<ProbationCriterionSnapshot>>([
                new("CRIT-WORK", "Work", 1)
            ]);

        public Task<bool> CriterionExistsAsync(string code, CancellationToken cancellationToken = default) =>
            Task.FromResult(string.Equals(code, "CRIT-WORK", StringComparison.OrdinalIgnoreCase));

        public Task<IReadOnlyList<ProbationExtendDurationSnapshot>> ListExtendDurationsAsync(
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<ProbationExtendDurationSnapshot>>([
                new("EXT-1M", "1 tháng", 1, 1)
            ]);

        public Task<ProbationExtendDurationSnapshot?> FindExtendDurationAsync(
            string code,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(
                string.Equals(code, "EXT-1M", StringComparison.OrdinalIgnoreCase)
                    ? new ProbationExtendDurationSnapshot("EXT-1M", "1 tháng", 1, 1)
                    : null);
    }

    private sealed class FakeEvalRepo : IProbationEvaluationRepository
    {
        public ProbationEvaluationSnapshot? Last { get; private set; }

        public Task<ProbationEvaluationSnapshot?> FindOpenByEmployeeAsync(
            Guid employeeId,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(Last);

        public Task<ProbationEvaluationSnapshot?> FindByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(Last);

        public Task<IReadOnlyList<ProbationEvaluationSnapshot>> ListAsync(
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<ProbationEvaluationSnapshot>>(
                Last is null ? [] : [Last]);

        public Task<ProbationEvaluationSnapshot> UpsertProposeAsync(
            Guid employeeId,
            string employeeCode,
            DateOnly probationEndDate,
            string outcomeCode,
            string proposedByIdpSubject,
            string? note,
            string? criteriaPayloadJson,
            CancellationToken cancellationToken = default)
        {
            Last = new ProbationEvaluationSnapshot(
                Guid.NewGuid(), employeeId, employeeCode, probationEndDate,
                ProbationEvaluationStatus.Proposed, outcomeCode, proposedByIdpSubject, DateTime.UtcNow, note,
                criteriaPayloadJson, null, null, null, null, null);
            return Task.FromResult(Last);
        }

        public Task<ProbationEvaluationSnapshot> DecideAsync(
            Guid employeeId,
            string employeeCode,
            DateOnly probationEndDate,
            string outcomeCode,
            string decidedByIdpSubject,
            string? note,
            string? extendDurationCode,
            string? criteriaPayloadJson,
            CancellationToken cancellationToken = default)
        {
            Last = new ProbationEvaluationSnapshot(
                Guid.NewGuid(), employeeId, employeeCode, probationEndDate,
                ProbationEvaluationStatus.Decided, null, null, null, null, criteriaPayloadJson,
                outcomeCode, decidedByIdpSubject, DateTime.UtcNow, note, extendDurationCode);
            return Task.FromResult(Last);
        }
    }
}
