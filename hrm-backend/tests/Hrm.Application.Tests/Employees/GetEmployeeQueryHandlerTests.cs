using Hrm.Application.Employees;
using Hrm.Application.Employees.Queries;
using Hrm.Domain.Employees;
using Hrm.Domain.Employees.Repositories;
using Hrm.Domain.Identity;
using Hrm.Domain.Identity.Repositories;
using Jarvis.Domain.Shared.ExceptionHandling;

namespace Hrm.Application.Tests.Employees;

public sealed class GetEmployeeQueryHandlerTests
{
    private static readonly Guid EmployeeId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");

    [Fact]
    public async Task HandleAsync_NvCanReadOwnProfile()
    {
        var handler = new GetEmployeeQueryHandler(
            new FakeAccountRepo(NvActor),
            new FakeEmployeeRepo(DevEmployee),
            new EmployeeDtoFactory(new FakeSeniorityRuleRepo()));

        var result = await handler.HandleAsync(
            new GetEmployeeQuery(EmployeeId, "local-dev"));

        Assert.Equal("MNV-DEV", result.EmployeeCode);
    }

    [Fact]
    public async Task HandleAsync_NvCannotReadOtherProfile()
    {
        var handler = new GetEmployeeQueryHandler(
            new FakeAccountRepo(NvActor),
            new FakeEmployeeRepo(OtherEmployee),
            new EmployeeDtoFactory(new FakeSeniorityRuleRepo()));

        await Assert.ThrowsAsync<ForbiddenException>(() =>
            handler.HandleAsync(new GetEmployeeQuery(EmployeeId, "local-dev")));
    }

    private static readonly IdentityAccountSnapshot NvActor = new(
        Guid.NewGuid(), "local-dev", "Dev", null, "MNV-DEV",
        IdentityAccountStatus.Active, ["IAM-ROLE-NV"]);

    private static readonly EmployeeSnapshot DevEmployee =
        EmpTestSnapshots.DevEmployee(EmployeeId);

    private static readonly EmployeeSnapshot OtherEmployee =
        EmpTestSnapshots.DevEmployee(EmployeeId, "MNV-OTHER", "Other", null, null);

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

    private sealed class FakeEmployeeRepo(EmployeeSnapshot snapshot) : IEmployeeReadRepository
    {
        public Task<IReadOnlyList<EmployeeSnapshot>> ListAsync(CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<EmployeeSnapshot>>([snapshot]);

        public Task<EmployeeSnapshot?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => Task.FromResult<EmployeeSnapshot?>(snapshot.Id == id ? snapshot : null);

        public Task<EmployeeSnapshot?> FindByEmployeeCodeAsync(
            string employeeCode,
            CancellationToken cancellationToken = default)
            => Task.FromResult<EmployeeSnapshot?>(
                string.Equals(snapshot.EmployeeCode, employeeCode, StringComparison.OrdinalIgnoreCase)
                    ? snapshot
                    : null);

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
            => Task.FromResult<EmployeeUniqueField?>(null);
    }
}
