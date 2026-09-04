using Hrm.Application.Probation.Commands;
using Hrm.Domain.Employees;
using Hrm.Domain.Employees.Repositories;
using Hrm.Domain.Identity;
using Hrm.Domain.Identity.Repositories;
using Hrm.Domain.Lifecycle;
using Hrm.Domain.Lifecycle.Repositories;
using Hrm.Domain.Probation;
using Hrm.Domain.Probation.Repositories;
using Jarvis.Domain.Shared.ExceptionHandling;

namespace Hrm.Application.Tests.Probation;

public sealed class DecideProbationEvaluationCommandHandlerTests
{
    private static readonly Guid EmpId = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee");

    [Fact]
    public async Task Decide_Hr_Pass_ConvertsContract()
    {
        var writes = new FakeWrites();
        var handler = NewHandler(writes, new FakeLif());
        var dto = await handler.HandleAsync(new DecideProbationEvaluationCommand(
            "local-dev", EmpId, ProbationOutcomeCodes.Pass, "OK", null, null));

        Assert.True(dto.ContractConvertedToOfficial);
        Assert.NotNull(writes.LastPatch?.Contract);
        Assert.Equal("OFFICIAL", writes.LastPatch!.Contract!.ContractType);
        Assert.False(writes.LastPatch.Contract.IsProbation);
    }

    [Fact]
    public async Task Decide_Extend_UpdatesKt()
    {
        var writes = new FakeWrites();
        var handler = NewHandler(writes, new FakeLif());
        var dto = await handler.HandleAsync(new DecideProbationEvaluationCommand(
            "local-dev", EmpId, ProbationOutcomeCodes.Extend, null, "EXT-1M", null));

        Assert.Equal(new DateOnly(2026, 7, 30), dto.NewProbationEndDate);
        Assert.Equal(new DateOnly(2026, 7, 30), writes.LastPatch!.Contract!.EndDate);
    }

    [Fact]
    public async Task Decide_Fail_OpensLif_DoesNotDeleteEmp()
    {
        var writes = new FakeWrites();
        var lif = new FakeLif();
        var handler = NewHandler(writes, lif);
        var dto = await handler.HandleAsync(new DecideProbationEvaluationCommand(
            "local-dev", EmpId, ProbationOutcomeCodes.Fail, "out", null, null));

        Assert.NotNull(dto.LifOffboardingCaseId);
        Assert.Equal(DecideProbationEvaluationCommandHandler.LifSourcePrbFail, lif.Last!.Source);
        Assert.Null(writes.LastPatch); // không xóa / không đụng HĐ khi fail
    }

    [Fact]
    public async Task Decide_Lm_Forbidden()
    {
        var handler = new DecideProbationEvaluationCommandHandler(
            new FakeAccounts(["IAM-ROLE-LM"], "MNV-LM"),
            new FakeEmployees([TvEmp()]),
            new FakeWrites(),
            new FakeMasters(),
            new FakeEvalRepo(),
            new FakeLif(),
            new FakeAuditLogs());

        await Assert.ThrowsAsync<ForbiddenException>(() =>
            handler.HandleAsync(new DecideProbationEvaluationCommand(
                "local-lm", EmpId, ProbationOutcomeCodes.Pass, null, null, null)));
    }

    [Fact]
    public async Task Decide_InvalidOutcome_BadRequest()
    {
        var handler = NewHandler(new FakeWrites(), new FakeLif());
        await Assert.ThrowsAsync<BadRequestException>(() =>
            handler.HandleAsync(new DecideProbationEvaluationCommand(
                "local-dev", EmpId, "CONDITIONAL", null, null, null)));
    }

    private static DecideProbationEvaluationCommandHandler NewHandler(FakeWrites writes, FakeLif lif) =>
        new(
            new FakeAccounts(["IAM-ROLE-HR"], "MNV-HR"),
            new FakeEmployees([TvEmp()]),
            writes,
            new FakeMasters(),
            new FakeEvalRepo(),
            lif,
            new FakeAuditLogs());

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

    private sealed class FakeWrites : IEmployeeWriteRepository
    {
        public EmployeePatch? LastPatch { get; private set; }

        public Task<Guid> CreateAsync(EmployeeCreateModel model, CancellationToken cancellationToken = default) =>
            Task.FromResult(Guid.NewGuid());

        public Task<bool> UpdateAsync(Guid id, EmployeePatch patch, CancellationToken cancellationToken = default)
        {
            LastPatch = patch;
            return Task.FromResult(true);
        }

        public Task SetLineManagerAsync(
            Guid employeeId,
            Guid lineManagerEmployeeId,
            CancellationToken cancellationToken = default) =>
            Task.CompletedTask;
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
            Task.FromResult<IReadOnlyList<ProbationCriterionSnapshot>>([]);

        public Task<bool> CriterionExistsAsync(string code, CancellationToken cancellationToken = default) =>
            Task.FromResult(true);

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
        public Task<ProbationEvaluationSnapshot?> FindOpenByEmployeeAsync(
            Guid employeeId,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<ProbationEvaluationSnapshot?>(null);

        public Task<ProbationEvaluationSnapshot?> FindByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<ProbationEvaluationSnapshot?>(null);

        public Task<IReadOnlyList<ProbationEvaluationSnapshot>> ListAsync(
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<ProbationEvaluationSnapshot>>([]);

        public Task<ProbationEvaluationSnapshot> UpsertProposeAsync(
            Guid employeeId,
            string employeeCode,
            DateOnly probationEndDate,
            string outcomeCode,
            string proposedByIdpSubject,
            string? note,
            string? criteriaPayloadJson,
            CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();

        public Task<ProbationEvaluationSnapshot> DecideAsync(
            Guid employeeId,
            string employeeCode,
            DateOnly probationEndDate,
            string outcomeCode,
            string decidedByIdpSubject,
            string? note,
            string? extendDurationCode,
            string? criteriaPayloadJson,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(new ProbationEvaluationSnapshot(
                Guid.NewGuid(), employeeId, employeeCode, probationEndDate,
                ProbationEvaluationStatus.Decided, null, null, null, null, criteriaPayloadJson,
                outcomeCode, decidedByIdpSubject, DateTime.UtcNow, note, extendDurationCode));
    }

    private sealed class FakeAuditLogs : IEmpAuditLogRepository
    {
        public Task AppendAsync(EmpAuditLogEntry entry, CancellationToken cancellationToken = default) =>
            Task.CompletedTask;

        public Task<IReadOnlyList<EmpAuditLogSnapshot>> ListByEmployeeIdAsync(
            Guid employeeId,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<EmpAuditLogSnapshot>>([]);
    }

    private sealed class FakeLif : ILifOffboardingRepository
    {
        public LifOffboardingCreateModel? Last { get; private set; }

        public Task<LifOffboardingSnapshot> CreateAsync(
            LifOffboardingCreateModel model,
            CancellationToken cancellationToken = default)
        {
            Last = model;
            return Task.FromResult(new LifOffboardingSnapshot(
                Guid.NewGuid(), model.EmployeeId, model.EmployeeCode, model.Source,
                LifOffboardingStatus.Open, null, model.ResignationSignedDate, null, null,
                DateTime.UtcNow, model.CreatedByIdpSubject, model.Note));
        }

        public Task<IReadOnlyList<LifOffboardingSnapshot>> ListOpenAsync(
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<LifOffboardingSnapshot>>([]);

        public Task<IReadOnlyList<LifOffboardingSnapshot>> ListAsync(
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<LifOffboardingSnapshot>>([]);

        public Task<LifOffboardingSnapshot?> FindByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<LifOffboardingSnapshot?>(null);

        public Task<LifOffboardingSnapshot> ConfirmNAsync(
            Guid id,
            DateOnly lastWorkingDayN,
            string confirmedByIdpSubject,
            CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();

        public Task<LifOffboardingSnapshot> CloseAsync(
            Guid id,
            string closedByIdpSubject,
            CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();

        public Task<LifOffboardingSnapshot> ApplyAccessLocksAsync(
            LifAccessLockApplyModel model,
            CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();
    }
}
