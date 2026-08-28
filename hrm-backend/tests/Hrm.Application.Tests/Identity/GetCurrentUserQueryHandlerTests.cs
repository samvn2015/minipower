using Hrm.Application.Identity;
using Hrm.Application.Identity.Queries;
using Hrm.Domain.Identity;
using Hrm.Domain.Identity.Repositories;

namespace Hrm.Application.Tests.Identity;

public sealed class GetCurrentUserQueryHandlerTests
{
    [Fact]
    public async Task HandleAsync_MapsRolesFromIamDb()
    {
        var snapshot = new IdentityAccountSnapshot(
            Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
            "sub-1",
            "Hùng IAM",
            "hung@company.local",
            "MNV001",
            IdentityAccountStatus.Active,
            ["IAM-ROLE-NV", "IAM-ROLE-HR"]);

        var handler = CreateHandler(new FakeIdentityAccountReadRepository(snapshot));

        var result = await handler.HandleAsync(
            new GetCurrentUserQuery("sub-1", "Token Name", "hung@company.local", ["ignored-from-token"]));

        Assert.Equal("sub-1", result.Sub);
        Assert.Equal("Hùng IAM", result.Name);
        Assert.Equal(2, result.Roles.Count);
        Assert.Contains("IAM-ROLE-NV", result.Roles);
        Assert.Contains("IAM-ROLE-HR", result.Roles);
        Assert.Null(result.Note);
    }

    [Fact]
    public async Task HandleAsync_DisabledAccount_ReturnsEmptyRoles()
    {
        var handler = CreateHandler(new FakeIdentityAccountReadRepository(
            new IdentityAccountSnapshot(
                Guid.NewGuid(),
                "sub-disabled",
                "Disabled User",
                null,
                null,
                IdentityAccountStatus.Disabled,
                ["IAM-ROLE-NV"])));

        var result = await handler.HandleAsync(
            new GetCurrentUserQuery("sub-disabled", null, null, []));

        Assert.Empty(result.Roles);
        Assert.Contains("vô hiệu", result.Note, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task HandleAsync_UnknownSubject_ReturnsProvisionerNote()
    {
        var handler = CreateHandler(new FakeIdentityAccountReadRepository(null));

        var result = await handler.HandleAsync(
            new GetCurrentUserQuery("unknown-sub", "X", null, ["Admin"]));

        Assert.Empty(result.Roles);
        Assert.Contains("email", result.Note, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task HandleAsync_NullQuery_ThrowsArgumentNullException()
    {
        var handler = CreateHandler(new FakeIdentityAccountReadRepository(null));
        await Assert.ThrowsAsync<ArgumentNullException>(
            () => handler.HandleAsync(null!));
    }

    private static GetCurrentUserQueryHandler CreateHandler(IIdentityAccountReadRepository accounts) =>
        new(accounts, new IdentityAccountProvisioner(
            accounts,
            new NoOpWriteRepository(),
            new NoOpEmployeeRepository()));

    private sealed class FakeIdentityAccountReadRepository(IdentityAccountSnapshot? snapshot)
        : IIdentityAccountReadRepository
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

    private sealed class NoOpWriteRepository : IIdentityAccountWriteRepository
    {
        public Task<IdentityAccountSnapshot> CreateAsync(
            IdentityAccountCreateModel model,
            CancellationToken cancellationToken = default)
            => throw new InvalidOperationException("unexpected create");
    }

    private sealed class NoOpEmployeeRepository : Domain.Employees.Repositories.IEmployeeReadRepository
    {
        public Task<IReadOnlyList<Domain.Employees.Repositories.EmployeeSnapshot>> ListAsync(
            CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<Domain.Employees.Repositories.EmployeeSnapshot>>([]);

        public Task<Domain.Employees.Repositories.EmployeeSnapshot?> FindByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
            => Task.FromResult<Domain.Employees.Repositories.EmployeeSnapshot?>(null);

        public Task<Domain.Employees.Repositories.EmployeeSnapshot?> FindByEmployeeCodeAsync(
            string employeeCode,
            CancellationToken cancellationToken = default)
            => Task.FromResult<Domain.Employees.Repositories.EmployeeSnapshot?>(null);

        public Task<Domain.Employees.Repositories.EmployeeSnapshot?> FindByEmailCtyAsync(
            string emailCty,
            CancellationToken cancellationToken = default)
            => Task.FromResult<Domain.Employees.Repositories.EmployeeSnapshot?>(null);

        public Task<Domain.Employees.EmployeeUniqueField?> FindDuplicateAsync(
            string employeeCode,
            string? cccd,
            string? emailCty,
            string? taxId,
            Guid? excludeEmployeeId = null,
            CancellationToken cancellationToken = default)
            => Task.FromResult<Domain.Employees.EmployeeUniqueField?>(null);
    }
}
