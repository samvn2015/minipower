using Hrm.Application.Identity.Admin.Commands;
using Hrm.Application.Tests.Common;
using Hrm.Domain.Employees;
using Hrm.Domain.Employees.Repositories;
using Hrm.Domain.Identity;
using Hrm.Domain.Identity.Repositories;
using Jarvis.Domain.Shared.ExceptionHandling;

namespace Hrm.Application.Tests.Identity;

/// <summary>
/// Doc-review pass 4 M2/M7: audit IAM chỉ ghi khi có thay đổi THẬT; no-op (gán role đã có,
/// gỡ role không có, disable tài khoản ma) không được để lại dòng audit giả.
/// </summary>
public sealed class IdentityAccountAdminAuditTests
{
    private static readonly Guid TargetId = Guid.Parse("22222222-2222-2222-2222-222222222222");

    private static readonly IdentityAccountSnapshot ItActor = new(
        Guid.NewGuid(), "it-dev", "IT", null, null, IdentityAccountStatus.Active, ["IAM-ROLE-IT"]);

    [Fact]
    public async Task AssignRole_Changed_WritesOneAuditRowInsideScope()
    {
        var audit = new FakeAudit();
        var scope = new FakeAtomicScope();
        var handler = new AssignAccountRoleCommandHandler(
            new FakeReadRepo(ItActor), new FakeAdminRepo(changed: true), audit, scope);

        await handler.HandleAsync(new AssignAccountRoleCommand(TargetId, "IAM-ROLE-NV", "it-dev"));

        var entry = Assert.Single(audit.Entries);
        Assert.Equal(EmpAuditActions.IamRoleAssigned, entry.Action);
        Assert.Equal(TargetId, entry.RelatedId);
        Assert.Equal("Role=IAM-ROLE-NV", entry.Detail);
        Assert.Equal(1, scope.Completed);
    }

    [Fact]
    public async Task AssignRole_AlreadyHasRole_NoAudit()
    {
        var audit = new FakeAudit();
        var handler = new AssignAccountRoleCommandHandler(
            new FakeReadRepo(ItActor), new FakeAdminRepo(changed: false), audit, new FakeAtomicScope());

        await handler.HandleAsync(new AssignAccountRoleCommand(TargetId, "IAM-ROLE-NV", "it-dev"));

        Assert.Empty(audit.Entries);
    }

    [Fact]
    public async Task RemoveRole_Changed_WritesOneAuditRow()
    {
        var audit = new FakeAudit();
        var handler = new RemoveAccountRoleCommandHandler(
            new FakeReadRepo(ItActor), new FakeAdminRepo(changed: true), audit, new FakeAtomicScope());

        await handler.HandleAsync(new RemoveAccountRoleCommand(TargetId, "IAM-ROLE-NV", "it-dev"));

        var entry = Assert.Single(audit.Entries);
        Assert.Equal(EmpAuditActions.IamRoleRemoved, entry.Action);
    }

    [Fact]
    public async Task RemoveRole_RoleAbsent_NoAudit()
    {
        var audit = new FakeAudit();
        var handler = new RemoveAccountRoleCommandHandler(
            new FakeReadRepo(ItActor), new FakeAdminRepo(changed: false), audit, new FakeAtomicScope());

        await handler.HandleAsync(new RemoveAccountRoleCommand(TargetId, "IAM-ROLE-NV", "it-dev"));

        Assert.Empty(audit.Entries);
    }

    [Fact]
    public async Task Disable_UnknownAccount_404BeforeAnyAudit()
    {
        var audit = new FakeAudit();
        var handler = new DisableIdentityAccountCommandHandler(
            new FakeReadRepo(ItActor), new FakeAdminRepo(changed: true, exists: false), audit, new FakeAtomicScope());

        await Assert.ThrowsAsync<NotFoundException>(() =>
            handler.HandleAsync(new DisableIdentityAccountCommand(TargetId, "it-dev")));

        Assert.Empty(audit.Entries);
    }

    [Fact]
    public async Task Disable_AlreadyDisabled_NoAudit()
    {
        var audit = new FakeAudit();
        var handler = new DisableIdentityAccountCommandHandler(
            new FakeReadRepo(ItActor), new FakeAdminRepo(changed: false), audit, new FakeAtomicScope());

        await handler.HandleAsync(new DisableIdentityAccountCommand(TargetId, "it-dev"));

        Assert.Empty(audit.Entries);
    }

    private sealed class FakeReadRepo(IdentityAccountSnapshot actor) : IIdentityAccountReadRepository
    {
        public Task<IdentityAccountSnapshot?> FindByIdpSubjectAsync(string idpSubject, CancellationToken cancellationToken = default)
            => Task.FromResult(actor.IdpSubject == idpSubject ? actor : null);

        public Task<IdentityAccountSnapshot?> FindByEmployeeCodeAsync(string employeeCode, CancellationToken cancellationToken = default)
            => Task.FromResult<IdentityAccountSnapshot?>(null);
    }

    private sealed class FakeAdminRepo(bool changed, bool exists = true) : IIdentityAccountAdminRepository
    {
        public Task<bool> AssignRoleAsync(Guid accountId, string roleCode, CancellationToken cancellationToken = default)
            => Task.FromResult(changed);

        public Task<bool> RemoveRoleAsync(Guid accountId, string roleCode, CancellationToken cancellationToken = default)
            => Task.FromResult(changed);

        public Task<bool> SetStatusAsync(Guid accountId, IdentityAccountStatus status, CancellationToken cancellationToken = default)
            => Task.FromResult(changed);

        public Task<IdentityAccountSnapshot?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => Task.FromResult<IdentityAccountSnapshot?>(exists
                ? new IdentityAccountSnapshot(id, "target", "Target", null, null, IdentityAccountStatus.Active, [])
                : null);

        public Task<IReadOnlyList<IdentityAccountSnapshot>> ListAsync(CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<IdentityAccountSnapshot>>([]);
    }

    private sealed class FakeAudit : IIamAuditLogRepository
    {
        public List<EmpAuditLogEntry> Entries { get; } = [];

        public Task AppendAsync(EmpAuditLogEntry entry, CancellationToken cancellationToken = default)
        {
            Entries.Add(entry);
            return Task.CompletedTask;
        }

        public Task<IReadOnlyList<EmpAuditLogSnapshot>> ListByEmployeeIdAsync(Guid employeeId, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<EmpAuditLogSnapshot>>([]);

        public Task<IReadOnlyList<EmpAuditLogSnapshot>> ListByActionAsync(string action, int take = 50, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<EmpAuditLogSnapshot>>([]);
    }
}
