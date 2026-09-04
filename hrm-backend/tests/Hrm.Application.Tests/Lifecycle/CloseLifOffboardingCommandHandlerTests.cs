using Hrm.Application.Lifecycle.Commands;
using Hrm.Domain.Identity;
using Hrm.Domain.Identity.Repositories;
using Hrm.Domain.Lifecycle;
using Hrm.Domain.Lifecycle.Repositories;
using Jarvis.Domain.Shared.ExceptionHandling;

namespace Hrm.Application.Tests.Lifecycle;

public sealed class CloseLifOffboardingCommandHandlerTests
{
    private static readonly Guid CaseId = Guid.Parse("cccccccc-cccc-cccc-cccc-ccccccccccc1");

    [Fact]
    public async Task Close_MissingMust_BadRequest()
    {
        var handler = new CloseLifOffboardingCommandHandler(
            new FakeAccounts(["IAM-ROLE-HR"]),
            new FakeOff(LifOffboardingStatus.ConfirmedN),
            new FakeChecklist(allMust: false));

        await Assert.ThrowsAsync<BadRequestException>(() =>
            handler.HandleAsync(new CloseLifOffboardingCommand("local-dev", CaseId)));
    }

    [Fact]
    public async Task Close_AllMust_Succeeds()
    {
        var off = new FakeOff(LifOffboardingStatus.ConfirmedN);
        var handler = new CloseLifOffboardingCommandHandler(
            new FakeAccounts(["IAM-ROLE-HR"]),
            off,
            new FakeChecklist(allMust: true));

        var dto = await handler.HandleAsync(new CloseLifOffboardingCommand("local-dev", CaseId));
        Assert.Equal("Closed", dto.Status);
        Assert.True(off.Closed);
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

    private sealed class FakeOff(LifOffboardingStatus status) : ILifOffboardingRepository
    {
        public bool Closed { get; private set; }

        public Task<LifOffboardingSnapshot> CreateAsync(
            LifOffboardingCreateModel model,
            CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();

        public Task<IReadOnlyList<LifOffboardingSnapshot>> ListOpenAsync(
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<LifOffboardingSnapshot>>([]);

        public Task<IReadOnlyList<LifOffboardingSnapshot>> ListAsync(
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<LifOffboardingSnapshot>>([]);

        public Task<LifOffboardingSnapshot?> FindByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<LifOffboardingSnapshot?>(Snap(status));

        public Task<LifOffboardingSnapshot> ConfirmNAsync(
            Guid id,
            DateOnly lastWorkingDayN,
            string confirmedByIdpSubject,
            CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();

        public Task<LifOffboardingSnapshot> CloseAsync(
            Guid id,
            string closedByIdpSubject,
            CancellationToken cancellationToken = default)
        {
            Closed = true;
            return Task.FromResult(Snap(LifOffboardingStatus.Closed));
        }

        public Task<LifOffboardingSnapshot> ApplyAccessLocksAsync(
            LifAccessLockApplyModel model,
            CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();

        private static LifOffboardingSnapshot Snap(LifOffboardingStatus s) =>
            new(CaseId, Guid.NewGuid(), "MNV-X", "HR-MANUAL", s, new DateOnly(2026, 9, 30),
                null, "hr", DateTime.UtcNow, DateTime.UtcNow, "hr", null);
    }

    private sealed class FakeChecklist(bool allMust) : ILifOffChecklistRepository
    {
        public Task<IReadOnlyList<LifOffChecklistItemSnapshot>> ListActiveItemsAsync(
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<LifOffChecklistItemSnapshot>>([
                new("OFF-RETURN-LAPTOP", "Laptop", true, 1)
            ]);

        public Task<IReadOnlyList<LifOffChecklistTickSnapshot>> ListTicksAsync(
            Guid caseId,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<LifOffChecklistTickSnapshot>>([]);

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
