using Hrm.Application.Identity;
using Hrm.Domain.Employees;
using Hrm.Domain.Employees.Repositories;
using Hrm.Domain.Identity;
using Hrm.Domain.Identity.Constants;
using Hrm.Domain.Identity.Repositories;

namespace Hrm.Application.Tests.Identity;

public sealed class IdentityAccountProvisionerTests
{
    [Fact]
    public async Task TryProvisionAsync_MatchesEmployeeByEmail_AssignsNvRole()
    {
        var employee = new EmployeeSnapshot(
            Guid.NewGuid(),
            "MNV-NEW",
            "NV Mới",
            null,
            "new@company.local",
            null,
            "ORG-HQ",
            null,
            null,
            EmployeeStatus.Active);

        var writeRepo = new RecordingWriteRepository();
        var provisioner = new IdentityAccountProvisioner(
            new FakeReadRepository(null, null),
            writeRepo,
            new FakeEmployeeRepository(employee));

        var result = await provisioner.TryProvisionAsync(
            "lark-sub-99",
            "new@company.local",
            "Token Name");

        Assert.NotNull(result.Account);
        Assert.Null(result.Note);
        Assert.Equal(IamRoleCodes.Nv, result.Account!.RoleCodes.Single());
        Assert.Equal("MNV-NEW", result.Account.EmployeeCode);
        Assert.Single(writeRepo.Models);
        Assert.Equal("lark-sub-99", writeRepo.Models[0].IdpSubject);
    }

    [Fact]
    public async Task TryProvisionAsync_NoEmail_ReturnsNote()
    {
        var provisioner = new IdentityAccountProvisioner(
            new FakeReadRepository(null, null),
            new RecordingWriteRepository(),
            new FakeEmployeeRepository(null));

        var result = await provisioner.TryProvisionAsync("sub-x", null, null);

        Assert.Null(result.Account);
        Assert.Contains("email", result.Note, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task TryProvisionAsync_EmployeeAlreadyLinked_ReturnsNote()
    {
        var employee = new EmployeeSnapshot(
            Guid.NewGuid(),
            "MNV-LINKED",
            "Linked",
            null,
            "linked@company.local",
            null,
            "ORG-HQ",
            null,
            null,
            EmployeeStatus.Active);

        var existing = new IdentityAccountSnapshot(
            Guid.NewGuid(),
            "other-sub",
            "Other",
            "linked@company.local",
            "MNV-LINKED",
            IdentityAccountStatus.Active,
            [IamRoleCodes.Nv]);

        var provisioner = new IdentityAccountProvisioner(
            new FakeReadRepository(null, existing),
            new RecordingWriteRepository(),
            new FakeEmployeeRepository(employee));

        var result = await provisioner.TryProvisionAsync(
            "new-sub",
            "linked@company.local",
            null);

        Assert.Null(result.Account);
        Assert.Contains("MNV", result.Note, StringComparison.Ordinal);
    }

    private sealed class FakeReadRepository(
        IdentityAccountSnapshot? bySubject,
        IdentityAccountSnapshot? byEmployeeCode) : IIdentityAccountReadRepository
    {
        public Task<IdentityAccountSnapshot?> FindByIdpSubjectAsync(
            string idpSubject,
            CancellationToken cancellationToken = default)
            => Task.FromResult(
                bySubject is not null && bySubject.IdpSubject == idpSubject ? bySubject : null);

        public Task<IdentityAccountSnapshot?> FindByEmployeeCodeAsync(
            string employeeCode,
            CancellationToken cancellationToken = default)
            => Task.FromResult(
                byEmployeeCode is not null && byEmployeeCode.EmployeeCode == employeeCode
                    ? byEmployeeCode
                    : null);
    }

    private sealed class FakeEmployeeRepository(EmployeeSnapshot? employee) : IEmployeeReadRepository
    {
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
            => Task.FromResult(
                employee is not null
                && string.Equals(employee.EmailCty, emailCty, StringComparison.OrdinalIgnoreCase)
                    ? employee
                    : null);

        public Task<EmployeeUniqueField?> FindDuplicateAsync(
            string employeeCode,
            string? cccd,
            string? emailCty,
            string? taxId,
            Guid? excludeEmployeeId = null,
            CancellationToken cancellationToken = default)
            => Task.FromResult<EmployeeUniqueField?>(null);
    }

    private sealed class RecordingWriteRepository : IIdentityAccountWriteRepository
    {
        public List<IdentityAccountCreateModel> Models { get; } = [];

        public Task<IdentityAccountSnapshot> CreateAsync(
            IdentityAccountCreateModel model,
            CancellationToken cancellationToken = default)
        {
            Models.Add(model);
            return Task.FromResult(new IdentityAccountSnapshot(
                Guid.NewGuid(),
                model.IdpSubject,
                model.DisplayName,
                model.EmailCty,
                model.EmployeeCode,
                IdentityAccountStatus.Active,
                model.InitialRoleCodes.ToList()));
        }
    }
}
