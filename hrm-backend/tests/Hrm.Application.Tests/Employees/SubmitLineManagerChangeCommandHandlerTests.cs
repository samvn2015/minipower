using Hrm.Application.Employees.Commands;
using Hrm.Domain.Employees;
using Hrm.Domain.Employees.Repositories;
using Hrm.Domain.Identity;
using Hrm.Domain.Identity.Repositories;

namespace Hrm.Application.Tests.Employees;

public sealed class SubmitLineManagerChangeCommandHandlerTests
{
    [Fact]
    public async Task HandleAsync_HrSubmitsChange()
    {
        var employeeId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");
        var lmId = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc");
        var changes = new FakeChangeRepo();
        var handler = new SubmitLineManagerChangeCommandHandler(
            new FakeAccountRepo(),
            new FakeOrgUnitRepo(),
            new FakeEmployeeRepo(employeeId, lmId),
            changes);

        var result = await handler.HandleAsync(
            new SubmitLineManagerChangeCommand("local-dev", employeeId, lmId));

        Assert.Equal("Pending", result.Status);
        Assert.NotEqual(Guid.Empty, result.RequestId);
    }

    private sealed class FakeAccountRepo : IIdentityAccountReadRepository
    {
        public Task<IdentityAccountSnapshot?> FindByIdpSubjectAsync(
            string idpSubject,
            CancellationToken cancellationToken = default)
            => Task.FromResult<IdentityAccountSnapshot?>(new IdentityAccountSnapshot(
                Guid.NewGuid(), "local-dev", "HR", null, null,
                IdentityAccountStatus.Active, ["IAM-ROLE-HR"]));

        public Task<IdentityAccountSnapshot?> FindByEmployeeCodeAsync(
            string employeeCode,
            CancellationToken cancellationToken = default)
            => Task.FromResult<IdentityAccountSnapshot?>(null);
    }

    private sealed class FakeOrgUnitRepo : IOrgUnitReadRepository
    {
        public Task<bool> IsActiveAsync(string orgUnitCode, CancellationToken cancellationToken = default)
            => Task.FromResult(true);
    }

    private sealed class FakeEmployeeRepo(Guid employeeId, Guid lmId) : IEmployeeReadRepository
    {
        public Task<IReadOnlyList<EmployeeSnapshot>> ListAsync(CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<EmployeeSnapshot>>([]);

        public Task<EmployeeSnapshot?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            if (id == employeeId)
            {
                return Task.FromResult<EmployeeSnapshot?>(new EmployeeSnapshot(
                    employeeId, "MNV-DEV", "Dev", null, null, null, "ORG-HQ", null, null, EmployeeStatus.Active));
            }

            if (id == lmId)
            {
                return Task.FromResult<EmployeeSnapshot?>(new EmployeeSnapshot(
                    lmId, "MNV-LM", "LM", null, null, null, "ORG-HQ", null, null, EmployeeStatus.Active));
            }

            return Task.FromResult<EmployeeSnapshot?>(null);
        }

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
            => Task.FromResult<EmployeeUniqueField?>(null);
    }

    private sealed class FakeChangeRepo : ILineManagerChangeRepository
    {
        public Task<LineManagerChangeSnapshot?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => Task.FromResult<LineManagerChangeSnapshot?>(null);

        public Task<LineManagerChangeSnapshot?> FindPendingByEmployeeIdAsync(
            Guid employeeId,
            CancellationToken cancellationToken = default)
            => Task.FromResult<LineManagerChangeSnapshot?>(null);

        public Task<IReadOnlyList<LineManagerChangeSnapshot>> ListPendingAsync(
            CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<LineManagerChangeSnapshot>>([]);

        public Task<Guid> CreateAsync(LineManagerChangeCreateModel model, CancellationToken cancellationToken = default)
            => Task.FromResult(Guid.NewGuid());

        public Task<bool> ApproveAsync(
            Guid requestId,
            Guid employeeId,
            Guid proposedLineManagerEmployeeId,
            string reviewedByIdpSubject,
            CancellationToken cancellationToken = default)
            => Task.FromResult(true);

        public Task<bool> RejectAsync(
            Guid requestId,
            string reviewedByIdpSubject,
            string? reviewNote,
            CancellationToken cancellationToken = default)
            => Task.FromResult(true);
    }
}
