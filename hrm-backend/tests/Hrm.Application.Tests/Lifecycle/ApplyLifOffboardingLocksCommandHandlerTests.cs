using Hrm.Application.Common;
using Hrm.Application.Lifecycle.Commands;
using Hrm.Domain.Employees;
using Hrm.Domain.Employees.Repositories;
using Hrm.Domain.Identity;
using Hrm.Domain.Identity.Repositories;
using Hrm.Domain.Lifecycle;
using Hrm.Domain.Lifecycle.Repositories;
using Jarvis.Domain.Shared.ExceptionHandling;

namespace Hrm.Application.Tests.Lifecycle;

public sealed class ApplyLifOffboardingLocksCommandHandlerTests
{
    private static readonly Guid CaseId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa2");
    private static readonly Guid EmpId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb2");
    private static readonly DateOnly N = new(2026, 9, 30);
    private static readonly DateOnly NPlus3 = new(2026, 10, 3);

    [Fact]
    public async Task It_LocksGitAndCrmSpTogether_AtNPlus3()
    {
        var repo = new FakeRepo();
        var audit = new FakeAudit();
        var handler = new ApplyLifOffboardingLocksCommandHandler(
            new FakeAccounts(["IAM-ROLE-IT"]),
            repo,
            audit);

        var dto = await handler.HandleAsync(
            new ApplyLifOffboardingLocksCommand("it-dev", CaseId, NPlus3, null));

        Assert.True(dto.GitLocked);
        Assert.True(dto.CrmSpLocked);
        Assert.NotNull(dto.LockedAtUtc);
        Assert.False(dto.IsEarlySecurityCr);
        Assert.Equal(LifOffboardingFacts.LockChannelGitAndCrmSp, repo.LastChannel);
        Assert.DoesNotContain("sales", repo.LastChannel, StringComparison.OrdinalIgnoreCase);
        Assert.Contains(audit.Entries, e =>
            e.Action == EmpAuditActions.LifOffboardingAccessLocked
            && e.Detail != null
            && e.Detail.Contains("nPlus3", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task EarlyWithoutCr_BadRequest()
    {
        var handler = new ApplyLifOffboardingLocksCommandHandler(
            new FakeAccounts(["IAM-ROLE-IT"]),
            new FakeRepo(),
            new FakeAudit());

        await Assert.ThrowsAsync<BadRequestException>(() =>
            handler.HandleAsync(
                new ApplyLifOffboardingLocksCommand("it-dev", CaseId, new DateOnly(2026, 10, 1), null)));
    }

    [Fact]
    public async Task EarlyWithCr_OkAndAudited()
    {
        var repo = new FakeRepo();
        var audit = new FakeAudit();
        var handler = new ApplyLifOffboardingLocksCommandHandler(
            new FakeAccounts(["IAM-ROLE-IT"]),
            repo,
            audit);

        var dto = await handler.HandleAsync(
            new ApplyLifOffboardingLocksCommand(
                "it-dev", CaseId, new DateOnly(2026, 10, 1), "CR-SEC-1 an ninh"));

        Assert.True(dto.GitLocked && dto.CrmSpLocked);
        Assert.True(dto.IsEarlySecurityCr);
        Assert.Equal("CR-SEC-1 an ninh", dto.EarlyCrReason);
        Assert.Contains(audit.Entries, e =>
            e.Action == EmpAuditActions.LifOffboardingAccessLocked
            && e.Detail != null
            && e.Detail.Contains("CR-SEC-1", StringComparison.Ordinal));
    }

    [Fact]
    public async Task Hr_Forbidden()
    {
        var handler = new ApplyLifOffboardingLocksCommandHandler(
            new FakeAccounts(["IAM-ROLE-HR"]),
            new FakeRepo(),
            new FakeAudit());

        await Assert.ThrowsAsync<ForbiddenException>(() =>
            handler.HandleAsync(
                new ApplyLifOffboardingLocksCommand("local-dev", CaseId, NPlus3, null)));
    }

    [Fact]
    public async Task Job_SkipsBeforeNPlus3_LocksWhenDue()
    {
        var repo = new FakeRepo();
        var audit = new FakeAudit();
        var handler = new RunLifNPlus3LocksCommandHandler(
            new FakeAccounts(["IAM-ROLE-IT"]),
            repo,
            new FakeHostRoleGate(active: true),
            audit);

        var early = await handler.HandleAsync(
            new RunLifNPlus3LocksCommand("it-dev", new DateOnly(2026, 10, 1)));
        Assert.Equal(0, early.Locked);
        Assert.Equal(1, early.SkippedNotDue);
        Assert.Empty(audit.Entries);

        var due = await handler.HandleAsync(
            new RunLifNPlus3LocksCommand("it-dev", NPlus3));
        Assert.Equal(1, due.Locked);
        Assert.True(repo.LastApply is not null);
        Assert.Contains(audit.Entries, e => e.Action == EmpAuditActions.LifOffboardingAccessLocked);
    }

    private sealed class FakeHostRoleGate(bool active) : IHostRoleGate
    {
        public bool IsActiveHost() => active;
    }

    private sealed class FakeAudit : IEmpAuditLogRepository
    {
        public List<EmpAuditLogEntry> Entries { get; } = [];

        public Task AppendAsync(EmpAuditLogEntry entry, CancellationToken cancellationToken = default)
        {
            Entries.Add(entry);
            return Task.CompletedTask;
        }

        public Task<IReadOnlyList<EmpAuditLogSnapshot>> ListByEmployeeIdAsync(
            Guid employeeId,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<EmpAuditLogSnapshot>>([]);

        public Task<IReadOnlyList<EmpAuditLogSnapshot>> ListByActionAsync(
            string action,
            int take = 50,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<EmpAuditLogSnapshot>>([]);

    }

    private sealed class FakeAccounts(string[] roles) : IIdentityAccountReadRepository
    {
        public Task<IdentityAccountSnapshot?> FindByIdpSubjectAsync(
            string idpSubject,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IdentityAccountSnapshot?>(new IdentityAccountSnapshot(
                Guid.NewGuid(), idpSubject, idpSubject, null, null,
                IdentityAccountStatus.Active, roles));

        public Task<IdentityAccountSnapshot?> FindByEmployeeCodeAsync(
            string employeeCode,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IdentityAccountSnapshot?>(null);
    }

    private sealed class FakeRepo : ILifOffboardingRepository
    {
        public (DateTime LockedAt, bool Early, string? Cr)? LastApply { get; private set; }
        public string LastChannel { get; private set; } = "";
        private bool _locked;

        public Task<LifOffboardingSnapshot> CreateAsync(
            LifOffboardingCreateModel model,
            CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();

        public Task<IReadOnlyList<LifOffboardingSnapshot>> ListOpenAsync(
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<LifOffboardingSnapshot>>([]);

        public Task<IReadOnlyList<LifOffboardingSnapshot>> ListAsync(
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<LifOffboardingSnapshot>>([Snap()]);

        public Task<LifOffboardingSnapshot?> FindByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<LifOffboardingSnapshot?>(Snap());

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
            CancellationToken cancellationToken = default)
        {
            _locked = true;
            var now = DateTime.UtcNow;
            LastApply = (now, model.IsEarlySecurityCr, model.CrReason);
            LastChannel = LifOffboardingFacts.LockChannelGitAndCrmSp;
            return Task.FromResult(Snap(now, model));
        }

        private LifOffboardingSnapshot Snap(DateTime? lockedAt = null, LifAccessLockApplyModel? m = null) =>
            new(
                CaseId, EmpId, "MNV-X", "HR-MANUAL", LifOffboardingStatus.ConfirmedN,
                N, null, "hr", DateTime.UtcNow, DateTime.UtcNow, "hr", null,
                _locked || lockedAt.HasValue ? lockedAt ?? DateTime.UtcNow : null,
                _locked || lockedAt.HasValue ? lockedAt ?? DateTime.UtcNow : null,
                m?.AsOfDate,
                m?.IsEarlySecurityCr ?? false,
                m?.CrReason,
                m?.LockedByIdpSubject);
    }
}
