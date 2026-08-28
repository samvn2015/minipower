using Hrm.Application.Employees.Queries;
using Hrm.Domain.Employees;
using Hrm.Domain.Employees.Repositories;
using Hrm.Domain.Identity;
using Hrm.Domain.Identity.Repositories;
using Jarvis.Domain.Shared.ExceptionHandling;

namespace Hrm.Application.Tests.Employees;

public sealed class ListEmployeesQueryHandlerTests
{
    [Fact]
    public async Task HandleAsync_HrReturnsList()
    {
        var handler = new ListEmployeesQueryHandler(
            new FakeAccountRepo(HrActor),
            new FakeEmployeeRepo([DevEmployee]));

        var result = await handler.HandleAsync(new ListEmployeesQuery("local-dev"));

        Assert.Single(result);
        Assert.Equal("MNV-DEV", result[0].EmployeeCode);
    }

    [Fact]
    public async Task HandleAsync_NvForbidden()
    {
        var handler = new ListEmployeesQueryHandler(
            new FakeAccountRepo(NvActor),
            new FakeEmployeeRepo([]));

        await Assert.ThrowsAsync<ForbiddenException>(() =>
            handler.HandleAsync(new ListEmployeesQuery("local-dev")));
    }

    private static readonly IdentityAccountSnapshot HrActor = new(
        Guid.NewGuid(), "local-dev", "Dev", null, "MNV-DEV",
        IdentityAccountStatus.Active, ["IAM-ROLE-HR"]);

    private static readonly IdentityAccountSnapshot NvActor = new(
        Guid.NewGuid(), "local-dev", "Dev", null, "MNV-DEV",
        IdentityAccountStatus.Active, ["IAM-ROLE-NV"]);

    private static readonly EmployeeSnapshot DevEmployee = new(
        Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
        "MNV-DEV", "Dev IAM", null, "dev@company.local", null, null, EmployeeStatus.Active);

    private sealed class FakeAccountRepo(IdentityAccountSnapshot snapshot) : IIdentityAccountReadRepository
    {
        public Task<IdentityAccountSnapshot?> FindByIdpSubjectAsync(
            string idpSubject,
            CancellationToken cancellationToken = default)
            => Task.FromResult<IdentityAccountSnapshot?>(
                snapshot.IdpSubject == idpSubject ? snapshot : null);
    }

    private sealed class FakeEmployeeRepo(IReadOnlyList<EmployeeSnapshot> items) : IEmployeeReadRepository
    {
        public Task<IReadOnlyList<EmployeeSnapshot>> ListAsync(CancellationToken cancellationToken = default)
            => Task.FromResult(items);

        public Task<EmployeeSnapshot?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => Task.FromResult(items.FirstOrDefault(e => e.Id == id));

        public Task<EmployeeSnapshot?> FindByEmployeeCodeAsync(
            string employeeCode,
            CancellationToken cancellationToken = default)
            => Task.FromResult(items.FirstOrDefault(e =>
                string.Equals(e.EmployeeCode, employeeCode, StringComparison.OrdinalIgnoreCase)));

        public Task<EmployeeUniqueField?> FindDuplicateAsync(
            string employeeCode,
            string? cccd,
            string? emailCty,
            string? taxId,
            Guid? excludeEmployeeId = null,
            CancellationToken cancellationToken = default)
            => Task.FromResult<EmployeeUniqueField?>(null);
    }
}
