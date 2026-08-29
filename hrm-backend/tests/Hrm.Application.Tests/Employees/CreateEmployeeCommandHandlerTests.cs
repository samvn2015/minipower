using Hrm.Application.Employees.Commands;
using Hrm.Domain.Employees;
using Hrm.Domain.Employees.Repositories;
using Hrm.Domain.Identity;
using Hrm.Domain.Identity.Repositories;
using Jarvis.Domain.Shared.ExceptionHandling;

namespace Hrm.Application.Tests.Employees;

public sealed class CreateEmployeeCommandHandlerTests
{
    [Fact]
    public async Task HandleAsync_NvCannotCreate()
    {
        var handler = new CreateEmployeeCommandHandler(
            new FakeAccountRepo(NvActor),
            new FakeOrgUnitRepo(),
            new FakeEmployeeRepo(),
            new FakeEmployeeWriteRepo());

        await Assert.ThrowsAsync<ForbiddenException>(() =>
            handler.HandleAsync(new CreateEmployeeCommand(
                "local-dev", "MNV-NEW", "X", null, null, null, "ORG-HQ", null)));
    }

    [Fact]
    public async Task HandleAsync_DuplicateCode_ThrowsConflict()
    {
        var read = new FakeEmployeeRepo { Duplicate = EmployeeUniqueField.EmployeeCode };
        var handler = new CreateEmployeeCommandHandler(
            new FakeAccountRepo(HrActor),
            new FakeOrgUnitRepo(),
            read,
            new FakeEmployeeWriteRepo());

        await Assert.ThrowsAsync<ConflictException>(() =>
            handler.HandleAsync(new CreateEmployeeCommand(
                "local-dev", "MNV-DEV", "X", null, null, null, "ORG-HQ", null)));
    }

    [Fact]
    public async Task HandleAsync_HrCreatesEmployee_WithContractWarningWhenMissing()
    {
        var write = new FakeEmployeeWriteRepo();
        var handler = new CreateEmployeeCommandHandler(
            new FakeAccountRepo(HrActor),
            new FakeOrgUnitRepo(),
            new FakeEmployeeRepo(),
            write);

        var result = await handler.HandleAsync(
            new CreateEmployeeCommand(
                "local-dev", "MNV-NEW", "New NV", null, "new@test.local", null, "ORG-HQ", null));

        Assert.Equal("MNV-NEW", result.EmployeeCode);
        Assert.NotEqual(Guid.Empty, write.LastCreatedId);
        Assert.Contains(result.Warnings, w => w.Contains("HĐ", StringComparison.Ordinal));
    }

    private static readonly IdentityAccountSnapshot HrActor = new(
        Guid.NewGuid(), "local-dev", "Dev", null, "MNV-DEV",
        IdentityAccountStatus.Active, ["IAM-ROLE-HR"]);

    private static readonly IdentityAccountSnapshot NvActor = new(
        Guid.NewGuid(), "local-dev", "Dev", null, "MNV-DEV",
        IdentityAccountStatus.Active, ["IAM-ROLE-NV"]);

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
            => Task.FromResult<IReadOnlyList<EmployeeSnapshot>>([]);

        public Task<EmployeeSnapshot?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => Task.FromResult<EmployeeSnapshot?>(null);

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
        public Guid LastCreatedId { get; private set; }

        public Task<Guid> CreateAsync(EmployeeCreateModel model, CancellationToken cancellationToken = default)
        {
            LastCreatedId = Guid.NewGuid();
            return Task.FromResult(LastCreatedId);
        }

        public Task<bool> UpdateAsync(Guid id, EmployeePatch patch, CancellationToken cancellationToken = default)
            => Task.FromResult(true);

        public Task SetLineManagerAsync(
            Guid employeeId,
            Guid lineManagerEmployeeId,
            CancellationToken cancellationToken = default)
            => Task.CompletedTask;
    }
}
