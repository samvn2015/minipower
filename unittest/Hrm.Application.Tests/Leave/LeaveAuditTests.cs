using Hrm.Application.Tests.Common;
using Hrm.Domain.Shared.Paging;
using Hrm.Application.Leave.Commands;
using Hrm.Application.Tests.Employees;
using Hrm.Domain.Employees;
using Hrm.Domain.Employees.Repositories;
using Hrm.Domain.Identity;
using Hrm.Domain.Identity.Repositories;
using Hrm.Domain.Leave;
using Hrm.Domain.Leave.Repositories;
using Jarvis.Domain.Shared.ExceptionHandling;

namespace Hrm.Application.Tests.Leave;

public sealed class LeaveAuditTests
{
    private static readonly Guid EmployeeId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");
    private static readonly Guid LmEmployeeId = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd");
    private static readonly Guid RequestId = Guid.Parse("99999999-9999-9999-9999-999999999999");

    // Doc-review pass 4 M7: 5 handler LEV/IAM chưa có test audit. Ba của LEV ở đây.

    [Fact]
    public async Task RejectC1_WritesAuditWithNoteInsideScope()
    {
        var audit = new FakeAudit();
        var scope = new FakeAtomicScope();
        var handler = new RejectLeaveRequestC1CommandHandler(
            new FakeAccountRepo("local-lm", "MNV-HO"),
            new FakeEmployeeRepo(),
            new FakeLeaveRequestRepo(),
            audit,
            scope);

        var result = await handler.HandleAsync(new RejectLeaveRequestC1Command("local-lm", RequestId, "thiếu bàn giao"));

        Assert.Equal("Rejected", result.Status);
        var entry = Assert.Single(audit.Entries);
        Assert.Equal(EmpAuditActions.LeaveRequestC1Rejected, entry.Action);
        Assert.Equal(EmployeeId, entry.EmployeeId);
        Assert.Equal(RequestId, entry.RelatedId);
        Assert.Equal("thiếu bàn giao", entry.Detail);
        Assert.Equal(1, scope.Completed);
    }

    [Fact]
    public async Task RejectC2_HrWritesAudit()
    {
        var audit = new FakeAudit();
        var handler = new RejectLeaveRequestC2CommandHandler(
            new FakeAccountRepo("local-dev", "MNV-DEV", ["IAM-ROLE-HR"]),
            new FakeLeaveRequestRepo(LeaveRequestStatus.PendingC2),
            audit,
            new FakeAtomicScope());

        var result = await handler.HandleAsync(new RejectLeaveRequestC2Command("local-dev", RequestId, null));

        Assert.Equal("Rejected", result.Status);
        var entry = Assert.Single(audit.Entries);
        Assert.Equal(EmpAuditActions.LeaveRequestC2Rejected, entry.Action);
        Assert.Equal("local-dev", entry.ActorIdpSubject);
    }

    [Fact]
    public async Task Cancel_OwnerWritesAuditWithFromStatus()
    {
        var audit = new FakeAudit();
        var handler = new CancelLeaveRequestCommandHandler(
            new FakeAccountRepo("local-dev", "MNV-DEV"),
            new FakeEmployeeRepo(),
            new FakeLeaveRequestRepo(LeaveRequestStatus.PendingC1),
            new FakeNotify(),
            audit,
            new FakeAtomicScope());

        var result = await handler.HandleAsync(new CancelLeaveRequestCommand("local-dev", RequestId));

        Assert.Equal("Cancelled", result.Status);
        var entry = Assert.Single(audit.Entries);
        Assert.Equal(EmpAuditActions.LeaveRequestCancelled, entry.Action);
        Assert.Equal(EmployeeId, entry.EmployeeId);
        Assert.Equal("FromStatus=PendingC1", entry.Detail);
    }

    [Fact]
    public async Task RejectC1_WrongState_NoAudit()
    {
        var audit = new FakeAudit();
        var handler = new RejectLeaveRequestC1CommandHandler(
            new FakeAccountRepo("local-lm", "MNV-HO"),
            new FakeEmployeeRepo(),
            new FakeLeaveRequestRepo(LeaveRequestStatus.PendingC2),
            audit,
            new FakeAtomicScope());

        await Assert.ThrowsAsync<ConflictException>(() =>
            handler.HandleAsync(new RejectLeaveRequestC1Command("local-lm", RequestId, null)));

        Assert.Empty(audit.Entries);
    }

    private sealed class FakeAccountRepo(string sub, string employeeCode, string[]? roles = null) : IIdentityAccountReadRepository
    {
        public Task<IdentityAccountSnapshot?> FindByIdpSubjectAsync(
            string idpSubject,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IdentityAccountSnapshot?>(new IdentityAccountSnapshot(
                Guid.NewGuid(),
                sub,
                sub,
                null,
                employeeCode,
                IdentityAccountStatus.Active,
                roles ?? ["IAM-ROLE-NV"]));

        public Task<IdentityAccountSnapshot?> FindByEmployeeCodeAsync(
            string employeeCode,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IdentityAccountSnapshot?>(null);
    }

    private sealed class FakeNotify : ILeaveNotificationOutbox
    {
        public Task PublishAsync(
            LeaveNotificationCreateModel model,
            CancellationToken cancellationToken = default) =>
            Task.CompletedTask;

        public Task<IReadOnlyList<LeaveNotificationSnapshot>> ListByEmployeeAsync(
            Guid employeeId,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<LeaveNotificationSnapshot>>([]);
    }

    private sealed class FakeEmployeeRepo : IEmployeeReadRepository
    {
        public Task<IReadOnlyList<EmployeeSnapshot>> ListAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<EmployeeSnapshot>>([]);

        public Task<PagedResult<EmployeeSnapshot>> ListPagedAsync(
            PageRequest page,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(new PagedResult<EmployeeSnapshot>([], 0));


        public Task<EmployeeSnapshot?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            if (id == EmployeeId)
            {
                return Task.FromResult<EmployeeSnapshot?>(
                    EmpTestSnapshots.DevEmployee(EmployeeId, lineManagerId: LmEmployeeId));
            }

            if (id == LmEmployeeId)
            {
                return Task.FromResult<EmployeeSnapshot?>(
                    EmpTestSnapshots.DevEmployee(LmEmployeeId, "MNV-HO", "Handover NV"));
            }

            return Task.FromResult<EmployeeSnapshot?>(null);
        }

        public Task<EmployeeSnapshot?> FindByEmployeeCodeAsync(
            string employeeCode,
            CancellationToken cancellationToken = default)
        {
            if (employeeCode == "MNV-DEV")
                return Task.FromResult<EmployeeSnapshot?>(
                    EmpTestSnapshots.DevEmployee(EmployeeId, lineManagerId: LmEmployeeId));
            if (employeeCode == "MNV-HO")
                return Task.FromResult<EmployeeSnapshot?>(
                    EmpTestSnapshots.DevEmployee(LmEmployeeId, "MNV-HO", "Handover NV"));
            return Task.FromResult<EmployeeSnapshot?>(null);
        }

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

    private sealed class FakeLeaveRequestRepo(LeaveRequestStatus status = LeaveRequestStatus.PendingC1) : ILeaveRequestRepository
    {
        public Task<Guid> CreateAsync(LeaveRequestCreateModel model, CancellationToken cancellationToken = default)
            => Task.FromResult(Guid.NewGuid());

        public Task<LeaveRequestSnapshot?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
            Task.FromResult<LeaveRequestSnapshot?>(new LeaveRequestSnapshot(
                RequestId,
                EmployeeId,
                "LEV-ANNUAL",
                "Phép năm",
                new DateOnly(2026, 12, 1),
                new DateOnly(2026, 12, 1),
                LeaveDayPart.FullDay,
                1m,
                "Test",
                LmEmployeeId,
                status,
                false,
                DateTime.UtcNow));

        public Task<IReadOnlyList<LeaveRequestSnapshot>> ListByEmployeeIdAsync(
            Guid employeeId,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<LeaveRequestSnapshot>>([]);

        public Task<IReadOnlyList<LeaveRequestPendingC1Snapshot>> ListPendingC1ByLineManagerIdAsync(
            Guid lineManagerEmployeeId,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<LeaveRequestPendingC1Snapshot>>([]);

        public Task<bool> ApproveC1Async(
            Guid id,
            string reviewedByIdpSubject,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(true);

        public Task<bool> RejectC1Async(
            Guid id,
            string reviewedByIdpSubject,
            string? reviewNote,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(true);

        public Task<IReadOnlyList<LeaveRequestPendingC1Snapshot>> ListPendingC2Async(
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<LeaveRequestPendingC1Snapshot>>([]);

        public Task<bool> ApproveC2Async(
            Guid id,
            string reviewedByIdpSubject,
            bool deductsAnnualBalance,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(true);

        public Task<bool> RejectC2Async(
            Guid id,
            string reviewedByIdpSubject,
            string? reviewNote,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(true);

        public Task<bool> HasOpenOverlapAsync(
            Guid employeeId,
            DateOnly fromDate,
            DateOnly toDate,
            LeaveDayPart dayPart,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(false);

        public Task<bool> CancelByEmployeeAsync(
            Guid id,
            Guid employeeId,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(true);

        public Task<IReadOnlyList<ApprovedLeaveForTimesheetSnapshot>> ListApprovedOverlappingPeriodAsync(
            string periodYm,
            IReadOnlyList<Guid> employeeIds,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<ApprovedLeaveForTimesheetSnapshot>>([]);
    }

    private sealed class FakeAudit : ILevAuditLogRepository
    {
        public List<EmpAuditLogEntry> Entries { get; } = [];

        public Task AppendAsync(EmpAuditLogEntry entry, CancellationToken cancellationToken = default)
        {
            Entries.Add(entry);
            return Task.CompletedTask;
        }

        public Task<IReadOnlyList<EmpAuditLogSnapshot>> ListByEmployeeIdAsync(
            Guid employeeId,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<EmpAuditLogSnapshot>>([]);

        public Task<IReadOnlyList<EmpAuditLogSnapshot>> ListByActionAsync(
            string action,
            int take = 50,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<EmpAuditLogSnapshot>>([]);
    }
}
