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
        var handler = CreateHandler(new FakeEmployeeRepo());

        await Assert.ThrowsAsync<ForbiddenException>(() =>
            handler.HandleAsync(new CreateEmployeeCommand(
                "local-dev", "MNV-NEW", "X", null, null, null, "ORG-HQ", null, null, null)));
    }

    [Fact]
    public async Task HandleAsync_DuplicateCode_ThrowsConflict()
    {
        var read = new FakeEmployeeRepo { Duplicate = EmployeeUniqueField.EmployeeCode };
        var handler = CreateHandler(read);

        await Assert.ThrowsAsync<ConflictException>(() =>
            handler.HandleAsync(new CreateEmployeeCommand(
                "local-dev", "MNV-DEV", "X", null, null, null, "ORG-HQ", null, null, null)));
    }

    [Fact]
    public async Task HandleAsync_DuplicateCccd_ThrowsConflict()
    {
        var read = new FakeEmployeeRepo { Duplicate = EmployeeUniqueField.Cccd };
        var handler = CreateHandler(read);

        var ex = await Assert.ThrowsAsync<ConflictException>(() =>
            handler.HandleAsync(new CreateEmployeeCommand(
                "local-dev", "MNV-NEW", "X", "012345678901", null, null, "ORG-HQ", null, null, null)));

        Assert.Contains("CCCD", ex.SystemMessage ?? ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public async Task HandleAsync_DuplicateEmail_ThrowsConflict()
    {
        var read = new FakeEmployeeRepo { Duplicate = EmployeeUniqueField.EmailCty };
        var handler = CreateHandler(read);

        var ex = await Assert.ThrowsAsync<ConflictException>(() =>
            handler.HandleAsync(new CreateEmployeeCommand(
                "local-dev", "MNV-NEW", "X", null, "dup@test.local", null, "ORG-HQ", null, null, null)));

        Assert.Contains("email", ex.SystemMessage ?? ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task HandleAsync_DuplicateTaxId_ThrowsConflict()
    {
        var read = new FakeEmployeeRepo { Duplicate = EmployeeUniqueField.TaxId };
        var handler = CreateHandler(read);

        await Assert.ThrowsAsync<ConflictException>(() =>
            handler.HandleAsync(new CreateEmployeeCommand(
                "local-dev", "MNV-NEW", "X", null, null, "MST-001", "ORG-HQ", null, null, null)));
    }

    [Fact]
    public async Task HandleAsync_InactiveOrg_ThrowsBadRequest()
    {
        var handler = CreateHandler(new FakeEmployeeRepo());

        var ex = await Assert.ThrowsAsync<BadRequestException>(() =>
            handler.HandleAsync(new CreateEmployeeCommand(
                "local-dev", "MNV-NEW", "X", null, null, null, "ORG-INACTIVE", null, null, null)));

        Assert.Contains("Org không hiệu lực", ex.SystemMessage ?? ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public async Task HandleAsync_InactiveEducation_ThrowsBadRequest()
    {
        var handler = CreateHandler(new FakeEmployeeRepo());

        var ex = await Assert.ThrowsAsync<BadRequestException>(() =>
            handler.HandleAsync(new CreateEmployeeCommand(
                "local-dev", "MNV-NEW", "X", null, null, null, "ORG-HQ", "EDU-INACTIVE", null, null)));

        Assert.Contains("học vấn", ex.SystemMessage ?? ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task HandleAsync_HrCreatesEmployee_WithContractWarningWhenMissing()
    {
        var write = new FakeEmployeeWriteRepo();
        var audit = new FakeAuditLogRepo();
        var handler = new CreateEmployeeCommandHandler(
            new FakeAccountRepo(HrActor),
            new FakeOrgUnitRepo(),
            new FakeEducationLevelRepo(),
            new FakeEmployeeRepo(),
            write,
            audit);

        var result = await handler.HandleAsync(
            new CreateEmployeeCommand(
                "local-dev",
                "MNV-NEW",
                "New NV",
                null,
                "new@test.local",
                null,
                "ORG-HQ",
                "EDU-DH",
                null,
                null));

        Assert.Equal("MNV-NEW", result.EmployeeCode);
        Assert.NotEqual(Guid.Empty, write.LastCreatedId);
        Assert.Contains(result.Warnings, w => w.Contains("HĐ", StringComparison.Ordinal));
        Assert.Contains(audit.Entries, e => e.Action == EmpAuditActions.EmployeeCreated);
    }

    private static CreateEmployeeCommandHandler CreateHandler(FakeEmployeeRepo read) =>
        new(
            new FakeAccountRepo(HrActor),
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
