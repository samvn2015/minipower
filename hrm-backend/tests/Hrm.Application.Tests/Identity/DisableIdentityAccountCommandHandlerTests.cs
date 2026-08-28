using Hrm.Application.Identity.Admin.Commands;
using Hrm.Domain.Identity;
using Hrm.Domain.Identity.Repositories;
using Jarvis.Domain.Shared.ExceptionHandling;

namespace Hrm.Application.Tests.Identity;

public sealed class DisableIdentityAccountCommandHandlerTests
{
    [Fact]
    public async Task HandleAsync_RequiresItRole()
    {
        var handler = new DisableIdentityAccountCommandHandler(
            new FakeReadRepo(HrActor),
            new FakeAdminRepo());

        await Assert.ThrowsAsync<ForbiddenException>(() =>
            handler.HandleAsync(new DisableIdentityAccountCommand(Guid.NewGuid(), "local-dev")));
    }

    [Fact]
    public async Task HandleAsync_ItUser_DisablesAccount()
    {
        var targetId = Guid.NewGuid();
        var admin = new FakeAdminRepo();
        var handler = new DisableIdentityAccountCommandHandler(
            new FakeReadRepo(ItActor),
            admin);

        var result = await handler.HandleAsync(
            new DisableIdentityAccountCommand(targetId, "it-dev"));

        Assert.Equal(targetId, result.AccountId);
        Assert.Equal(nameof(IdentityAccountStatus.Disabled), result.Status);
        Assert.Equal(IdentityAccountStatus.Disabled, admin.LastStatus);
    }

    private static readonly IdentityAccountSnapshot HrActor = new(
        Guid.NewGuid(), "local-dev", "Dev", null, null,
        IdentityAccountStatus.Active, ["IAM-ROLE-HR"]);

    private static readonly IdentityAccountSnapshot ItActor = new(
        Guid.NewGuid(), "it-dev", "IT", null, null,
        IdentityAccountStatus.Active, ["IAM-ROLE-IT"]);

    private sealed class FakeReadRepo(IdentityAccountSnapshot? snapshot) : IIdentityAccountReadRepository
    {
        public Task<IdentityAccountSnapshot?> FindByIdpSubjectAsync(
            string idpSubject,
            CancellationToken cancellationToken = default)
            => Task.FromResult(
                snapshot is not null && snapshot.IdpSubject == idpSubject ? snapshot : null);

        public Task<IdentityAccountSnapshot?> FindByEmployeeCodeAsync(
            string employeeCode,
            CancellationToken cancellationToken = default)
            => Task.FromResult<IdentityAccountSnapshot?>(null);
    }

    private sealed class FakeAdminRepo : IIdentityAccountAdminRepository
    {
        public IdentityAccountStatus? LastStatus { get; private set; }

        public Task AssignRoleAsync(Guid accountId, string roleCode, CancellationToken cancellationToken = default)
            => Task.CompletedTask;

        public Task<IdentityAccountSnapshot?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => Task.FromResult<IdentityAccountSnapshot?>(new IdentityAccountSnapshot(
                id, "target", "Target", null, null, IdentityAccountStatus.Disabled, []));

        public Task<IReadOnlyList<IdentityAccountSnapshot>> ListAsync(CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<IdentityAccountSnapshot>>([]);

        public Task RemoveRoleAsync(Guid accountId, string roleCode, CancellationToken cancellationToken = default)
            => Task.CompletedTask;

        public Task SetStatusAsync(Guid accountId, IdentityAccountStatus status, CancellationToken cancellationToken = default)
        {
            LastStatus = status;
            return Task.CompletedTask;
        }
    }
}
