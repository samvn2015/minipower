using Hrm.Application.Employees.Commands;
using Hrm.Domain.Employees;
using Hrm.Domain.Employees.Repositories;
using Hrm.Domain.Identity;
using Hrm.Domain.Identity.Repositories;
using Jarvis.Domain.Shared.ExceptionHandling;

namespace Hrm.Application.Tests.Employees;

public sealed class UpdateEmployeeCommandHandlerTests
{
    private static readonly Guid EmployeeId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");

    [Fact]
    public async Task HandleAsync_DuplicateEmailOnPatch_ThrowsConflict()
    {
        var handler = CreateHandler(new FakeEmployeeRepo { Duplicate = EmployeeUniqueField.EmailCty });

        await Assert.ThrowsAsync<ConflictException>(() =>
            handler.HandleAsync(new UpdateEmployeeCommand(
                EmployeeId, "local-dev", null, "other@test.local", null, null, null, null, null, null)));
    }

    [Fact]
    public async Task HandleAsync_InactiveOrgOnPatch_ThrowsBadRequest()
    {
        var handler = CreateHandler(new FakeEmployeeRepo());

        var ex = await Assert.ThrowsAsync<BadRequestException>(() =>
            handler.HandleAsync(new UpdateEmployeeCommand(
                EmployeeId, "local-dev", null, null, null, null, "ORG-INACTIVE", null, null, null)));

        Assert.Contains("Org không hiệu lực", ex.SystemMessage ?? ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public async Task HandleAsync_NvCannotPatchOrg()
    {
        var handler = CreateHandler(new FakeEmployeeRepo(), NvActor);

        await Assert.ThrowsAsync<ForbiddenException>(() =>
            handler.HandleAsync(new UpdateEmployeeCommand(
                EmployeeId, "local-dev", null, null, null, null, "ORG-HQ", null, null, null)));
    }

    [Fact]
    public async Task HandleAsync_NvCannotPatchEducation()
    {
        var handler = CreateHandler(new FakeEmployeeRepo(), NvActor);

        await Assert.ThrowsAsync<ForbiddenException>(() =>
            handler.HandleAsync(new UpdateEmployeeCommand(
                EmployeeId, "local-dev", null, null, null, null, null, "EDU-DH", null, null)));
    }

    private static UpdateEmployeeCommandHandler CreateHandler(
        FakeEmployeeRepo read,
        IdentityAccountSnapshot? actor = null) =>
        new(
            new FakeAccountRepo(actor ?? HrActor),
            new FakeOrgUnitRepo(),
            new FakeEducationLevelRepo(),
            read,
            new FakeEmployeeWriteRepo(),
            new FakeAuditLogRepo());

    private static readonly IdentityAccountSnapshot HrActor = new(
        Guid.NewGuid(), "local-dev", "Dev", null, "MNV-DEV",
        IdentityAccountStatus.Active, ["IAM-ROLE-HR"]);

    private static readonly IdentityAccountSnapshot NvActor = new(
        Guid.NewGuid(), "local-dev", "Dev", null, "MNV-DEV",
        IdentityAccountStatus.Active, ["IAM-ROLE-NV"]);

    private static readonly EmployeeSnapshot DevEmployee =
        EmpTestSnapshots.DevEmployee(EmployeeId);

    private sealed class FakeOrgUnitRepo : IOrgUnitReadRepository
    {
        public Task<bool> IsActiveAsync(string orgUnitCode, CancellationToken cancellationToken = default) =>
            Task.FromResult(string.Equals(orgUnitCode, "ORG-HQ", StringComparison.OrdinalIgnoreCase));
    }

    private sealed class FakeAccountRepo(IdentityAccountSnapshot snapshot) : IIdentityAccountReadRepository
    {
        public Task<IdentityAccountSnapshot?> FindByIdpSubjectAsync(
            string idpSubject,
            CancellationToken cancellationToken = default)
            => Task.FromResult<IdentityAccountSnapshot?>(
                snapshot.IdpSubject == idpSubject ? snapshot : null);

        public Task<IdentityAccountSnapshot?> FindByEmployeeCodeAsync(
            string employeeCode,
            CancellationToken cancellationToken = default)
            => Task.FromResult<IdentityAccountSnapshot?>(null);
    }

    private sealed class FakeEmployeeRepo : IEmployeeReadRepository
    {
        public EmployeeUniqueField? Duplicate { get; init; }

        public Task<IReadOnlyList<EmployeeSnapshot>> ListAsync(CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<EmployeeSnapshot>>([DevEmployee]);

        public Task<EmployeeSnapshot?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => Task.FromResult<EmployeeSnapshot?>(id == EmployeeId ? DevEmployee : null);

        public Task<EmployeeSnapshot?> FindByEmployeeCodeAsync(
            string employeeCode,
            CancellationToken cancellationToken = default)
            => Task.FromResult<EmployeeSnapshot?>(null);

        public Task<EmployeeSnapshot?> FindByEmailCtyAsync(
            string emailCty,
            CancellationToken cancellationToken = default)
            => Task.FromResult<EmployeeSnapshot?>(null);

        public Task<EmployeeUniqueField?> FindDuplicateAsync(
            string employeeCode,
            string? cccd,
            string? emailCty,
            string? taxId,
            Guid? excludeEmployeeId = null,
            CancellationToken cancellationToken = default)
            => Task.FromResult(Duplicate);
    }

    private sealed class FakeEmployeeWriteRepo : IEmployeeWriteRepository
    {
        public Task<Guid> CreateAsync(EmployeeCreateModel model, CancellationToken cancellationToken = default)
            => Task.FromResult(Guid.NewGuid());

        public Task<bool> UpdateAsync(Guid id, EmployeePatch patch, CancellationToken cancellationToken = default)
            => Task.FromResult(true);

        public Task SetLineManagerAsync(
            Guid employeeId,
            Guid lineManagerEmployeeId,
            CancellationToken cancellationToken = default)
            => Task.CompletedTask;
    }
}
