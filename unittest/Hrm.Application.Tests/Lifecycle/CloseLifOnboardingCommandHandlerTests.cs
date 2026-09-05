using Hrm.Application.Lifecycle.Commands;
using Hrm.Domain.Identity;
using Hrm.Domain.Identity.Repositories;
using Hrm.Domain.Lifecycle;
using Hrm.Domain.Lifecycle.Repositories;
using Jarvis.Domain.Shared.ExceptionHandling;

namespace Hrm.Application.Tests.Lifecycle;

public sealed class CloseLifOnboardingCommandHandlerTests
{
    private static readonly Guid CaseId = Guid.Parse("dddddddd-dddd-dddd-dddd-ddddddddddd1");

    [Fact]
    public async Task Close_MissingMust_BadRequest()
    {
        var handler = new CloseLifOnboardingCommandHandler(
            new FakeAccounts(["IAM-ROLE-HR"]),
            new FakeOn(),
            new FakeChecklist(allMust: false));

        await Assert.ThrowsAsync<BadRequestException>(() =>
            handler.HandleAsync(new CloseLifOnboardingCommand("local-dev", CaseId)));
    }

    [Fact]
    public async Task Close_MissingGitProvision_BadRequest()
    {
        var handler = new CloseLifOnboardingCommandHandler(
            new FakeAccounts(["IAM-ROLE-HR"]),
            new FakeOn(git: false),
            new FakeChecklist(allMust: true));

        await Assert.ThrowsAsync<BadRequestException>(() =>
            handler.HandleAsync(new CloseLifOnboardingCommand("local-dev", CaseId)));
    }

    [Fact]
    public async Task Close_AllMustAndProvisions_Ok()
    {
        var on = new FakeOn();
        var handler = new CloseLifOnboardingCommandHandler(
            new FakeAccounts(["IAM-ROLE-HR"]),
            on,
            new FakeChecklist(allMust: true));

        var dto = await handler.HandleAsync(new CloseLifOnboardingCommand("local-dev", CaseId));
        Assert.Equal("Closed", dto.Status);
        Assert.True(on.Closed);
    }

    [Fact]
    public async Task MarkProvision_DeferGitToNPlus3_BadRequest()
    {
        var handler = new MarkLifOnboardingProvisionedCommandHandler(
            new FakeAccounts(["IAM-ROLE-IT"]),
            new FakeOn());

        await Assert.ThrowsAsync<BadRequestException>(() =>
            handler.HandleAsync(
                new MarkLifOnboardingProvisionedCommand(
                    "it-dev", CaseId, LifProvisionSystems.Git, DeferGitToNPlus3: true)));
    }

    private sealed class FakeAccounts(string[] roles) : IIdentityAccountReadRepository
    {
        public Task<IdentityAccountSnapshot?> FindByIdpSubjectAsync(
            string idpSubject,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IdentityAccountSnapshot?>(new IdentityAccountSnapshot(
                Guid.NewGuid(), idpSubject, idpSubject, null, "MNV-HR",
                IdentityAccountStatus.Active, roles));

        public Task<IdentityAccountSnapshot?> FindByEmployeeCodeAsync(
            string employeeCode,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IdentityAccountSnapshot?>(null);
    }

    private sealed class FakeOn(bool git = true) : ILifOnboardingRepository
    {
        public bool Closed { get; private set; }

        public Task<LifOnboardingSnapshot> CreateAsync(
            LifOnboardingCreateModel model,
            CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();

        public Task<IReadOnlyList<LifOnboardingSnapshot>> ListAsync(
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<LifOnboardingSnapshot>>([]);

        public Task<LifOnboardingSnapshot?> FindByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<LifOnboardingSnapshot?>(Snap());

        public Task<LifOnboardingSnapshot> MarkProvisionedAsync(
            Guid id,
            string systemCode,
            string actorIdpSubject,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(Snap());

        public Task<LifOnboardingSnapshot> CloseAsync(
            Guid id,
            string closedByIdpSubject,
            CancellationToken cancellationToken = default)
        {
            Closed = true;
            return Task.FromResult(Snap(LifOnboardingStatus.Closed));
        }

        private LifOnboardingSnapshot Snap(LifOnboardingStatus s = LifOnboardingStatus.Open) =>
            new(
                CaseId, Guid.NewGuid(), "MNV-ON", s, DateTime.UtcNow, "hr", null,
                true, git, true, true,
                DateTime.UtcNow, git ? DateTime.UtcNow : null, DateTime.UtcNow, DateTime.UtcNow,
                s == LifOnboardingStatus.Closed ? "hr" : null,
                s == LifOnboardingStatus.Closed ? DateTime.UtcNow : null);
    }

    private sealed class FakeChecklist(bool allMust) : ILifOnChecklistRepository
    {
        public Task<IReadOnlyList<LifOnChecklistItemSnapshot>> ListActiveItemsAsync(
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<LifOnChecklistItemSnapshot>>([
                new("ON-PAPERWORK", "Paper", true, 1)
            ]);

        public Task<IReadOnlyList<LifOnChecklistTickSnapshot>> ListTicksAsync(
            Guid caseId,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<LifOnChecklistTickSnapshot>>([]);

        public Task UpsertTickAsync(
            Guid caseId,
            string itemCode,
            bool isChecked,
            string actorIdpSubject,
            CancellationToken cancellationToken = default) =>
            Task.CompletedTask;

        public Task<bool> AllMustCheckedAsync(Guid caseId, CancellationToken cancellationToken = default) =>
            Task.FromResult(allMust);
    }
}
