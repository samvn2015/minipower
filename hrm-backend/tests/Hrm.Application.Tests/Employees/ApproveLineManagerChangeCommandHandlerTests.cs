using Hrm.Application.Employees.Commands;
using Hrm.Domain.Employees;
using Hrm.Domain.Employees.Repositories;
using Hrm.Domain.Identity;
using Hrm.Domain.Identity.Repositories;
using Jarvis.Domain.Shared.ExceptionHandling;

namespace Hrm.Application.Tests.Employees;

public sealed class ApproveLineManagerChangeCommandHandlerTests
{
    private static readonly Guid RequestId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa9");
    private static readonly Guid EmpId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb9");
    private static readonly Guid LmId = Guid.Parse("cccccccc-cccc-cccc-cccc-ccccccccccc9");

    [Fact]
    public async Task Approve_WhenLmOrgInactive_BadRequest()
    {
        var handler = new ApproveLineManagerChangeCommandHandler(
            new FakeAccounts(),
            new FakeEmployees(lmOrg: "ORG-INACTIVE"),
            new FakeOrg(active: false),
            new FakeChanges(),
            new FakeAudit());

        await Assert.ThrowsAsync<BadRequestException>(() =>
            handler.HandleAsync(new ApproveLineManagerChangeCommand("local-dev", RequestId)));
    }

    [Fact]
    public async Task Approve_WhenLmOrgActive_Ok()
    {
        var changes = new FakeChanges();
        var handler = new ApproveLineManagerChangeCommandHandler(
            new FakeAccounts(),
            new FakeEmployees(lmOrg: "ORG-HQ"),
            new FakeOrg(active: true),
            changes,
            new FakeAudit());

        var result = await handler.HandleAsync(new ApproveLineManagerChangeCommand("local-dev", RequestId));
        Assert.Equal("Approved", result.Status);
        Assert.True(changes.Approved);
    }

    private sealed class FakeAccounts : IIdentityAccountReadRepository
    {
        public Task<IdentityAccountSnapshot?> FindByIdpSubjectAsync(
            string idpSubject,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IdentityAccountSnapshot?>(new IdentityAccountSnapshot(
                Guid.NewGuid(), idpSubject, idpSubject, null, null,
                IdentityAccountStatus.Active, ["IAM-ROLE-HR"]));

        public Task<IdentityAccountSnapshot?> FindByEmployeeCodeAsync(
            string employeeCode,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IdentityAccountSnapshot?>(null);
    }

    private sealed class FakeEmployees(string lmOrg) : IEmployeeReadRepository
    {
        public Task<IReadOnlyList<EmployeeSnapshot>> ListAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<EmployeeSnapshot>>([]);

        public Task<EmployeeSnapshot?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            if (id == LmId)
                return Task.FromResult<EmployeeSnapshot?>(
                    EmpTestSnapshots.DevEmployee(LmId, "MNV-LM", "LM", org: lmOrg));
            return Task.FromResult<EmployeeSnapshot?>(
                EmpTestSnapshots.DevEmployee(EmpId, "MNV-X", "NV"));
        }

        public Task<EmployeeSnapshot?> FindByEmployeeCodeAsync(
            string employeeCode,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<EmployeeSnapshot?>(null);

        public Task<EmployeeSnapshot?> FindByEmailCtyAsync(
            string emailCty,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<EmployeeSnapshot?>(null);

        public Task<EmployeeUniqueField?> FindDuplicateAsync(
            string employeeCode,
            string? cccd,
            string? emailCty,
            string? taxId,
            Guid? excludeEmployeeId = null,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<EmployeeUniqueField?>(null);
    }

    private sealed class FakeOrg(bool active) : IOrgUnitReadRepository
    {
        public Task<bool> IsActiveAsync(string orgUnitCode, CancellationToken cancellationToken = default) =>
            Task.FromResult(active);
    }

    private sealed class FakeChanges : ILineManagerChangeRepository
    {
        public bool Approved { get; private set; }

        public Task<LineManagerChangeSnapshot?> FindByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<LineManagerChangeSnapshot?>(new LineManagerChangeSnapshot(
                RequestId,
                EmpId,
                "MNV-X",
                "NV",
                LmId,
                "MNV-LM",
                "LM",
                LineManagerChangeStatus.Pending,
                "hr",
                DateTime.UtcNow,
                null,
                null,
                null));

        public Task<LineManagerChangeSnapshot?> FindPendingByEmployeeIdAsync(
            Guid employeeId,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<LineManagerChangeSnapshot?>(null);

        public Task<IReadOnlyList<LineManagerChangeSnapshot>> ListPendingAsync(
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<LineManagerChangeSnapshot>>([]);

        public Task<Guid> CreateAsync(
            LineManagerChangeCreateModel model,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(Guid.NewGuid());

        public Task<bool> ApproveAsync(
            Guid requestId,
            Guid employeeId,
            Guid proposedLineManagerEmployeeId,
            string reviewedByIdpSubject,
            CancellationToken cancellationToken = default)
        {
            Approved = true;
            return Task.FromResult(true);
        }

        public Task<bool> RejectAsync(
            Guid requestId,
            string reviewedByIdpSubject,
            string? reviewNote,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(true);
    }

    private sealed class FakeAudit : IEmpAuditLogRepository
    {
        public Task AppendAsync(EmpAuditLogEntry entry, CancellationToken cancellationToken = default) =>
            Task.CompletedTask;

        public Task<IReadOnlyList<EmpAuditLogSnapshot>> ListByEmployeeIdAsync(
            Guid employeeId,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<EmpAuditLogSnapshot>>([]);
    }
}
