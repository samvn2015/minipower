using Hrm.Application.Tests.Common;
using Hrm.Domain.Employees.Repositories;
using Hrm.Domain.Employees;
using Hrm.Application.Leave.Commands;
using Hrm.Domain.Identity;
using Hrm.Domain.Identity.Repositories;
using Hrm.Domain.Leave;
using Hrm.Domain.Leave.Repositories;
using Jarvis.Domain.Shared.ExceptionHandling;

namespace Hrm.Application.Tests.Leave;

public sealed class ApproveLeaveRequestC2CommandHandlerTests
{
    private static readonly Guid RequestId = Guid.Parse("88888888-8888-8888-8888-888888888888");

    [Fact]
    public async Task HandleAsync_HrApproves_MovesToApproved()
    {
        var audit = new FakeAudit();
        var handler = new ApproveLeaveRequestC2CommandHandler(
            new FakeAccountRepo("local-dev", ["IAM-ROLE-HR"]),
            new FakeLeaveRequestRepo(),
            new FakeLeaveTypeRepo(),
            new FakeNotify(),
            audit,
            new FakeAtomicScope());

        var result = await handler.HandleAsync(new ApproveLeaveRequestC2Command("local-dev", RequestId));

        Assert.Equal("Approved", result.Status);
        // NFR-005 (B1): C2 phải để lại dòng audit bất biến, ghi đúng actor và đơn.
        var entry = Assert.Single(audit.Entries);
        Assert.Equal(EmpAuditActions.LeaveRequestC2Approved, entry.Action);
        Assert.Equal(RequestId, entry.RelatedId);
        Assert.Equal("local-dev", entry.ActorIdpSubject);
    }

    [Fact]
    public async Task HandleAsync_LmCannotC2_ThrowsForbidden()
    {
        var audit = new FakeAudit();
        var handler = new ApproveLeaveRequestC2CommandHandler(
            new FakeAccountRepo("local-lm", ["IAM-ROLE-LM", "IAM-ROLE-NV"]),
            new FakeLeaveRequestRepo(),
            new FakeLeaveTypeRepo(),
            new FakeNotify(),
            audit,
            new FakeAtomicScope());

        await Assert.ThrowsAsync<ForbiddenException>(() =>
            handler.HandleAsync(new ApproveLeaveRequestC2Command("local-lm", RequestId)));
        Assert.Empty(audit.Entries);
    }

    private sealed class FakeAccountRepo(string sub, string[] roles) : IIdentityAccountReadRepository
    {
        public Task<IdentityAccountSnapshot?> FindByIdpSubjectAsync(
            string idpSubject,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IdentityAccountSnapshot?>(new IdentityAccountSnapshot(
                Guid.NewGuid(),
                sub,
                sub,
                null,
                null,
                IdentityAccountStatus.Active,
                roles));

        public Task<IdentityAccountSnapshot?> FindByEmployeeCodeAsync(
            string employeeCode,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IdentityAccountSnapshot?>(null);
    }

    private sealed class FakeLeaveTypeRepo : ILeaveTypeReadRepository
    {
        public Task<IReadOnlyList<LeaveTypeSnapshot>> ListActiveAsync(
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<LeaveTypeSnapshot>>([]);

        public Task<LeaveTypeSnapshot?> FindByCodeAsync(
            string code,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<LeaveTypeSnapshot?>(
                new LeaveTypeSnapshot(code, "Annual", true, false, LeaveTypeStatus.Active));
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

    private sealed class FakeLeaveRequestRepo : ILeaveRequestRepository
    {
        public Task<Guid> CreateAsync(LeaveRequestCreateModel model, CancellationToken cancellationToken = default)
            => Task.FromResult(Guid.NewGuid());

        public Task<LeaveRequestSnapshot?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
            Task.FromResult<LeaveRequestSnapshot?>(new LeaveRequestSnapshot(
                RequestId,
                Guid.NewGuid(),
                "LEV-ANNUAL",
                "Phép năm",
                new DateOnly(2026, 12, 15),
                new DateOnly(2026, 12, 15),
                LeaveDayPart.FullDay,
                1m,
                "Test",
                Guid.NewGuid(),
                LeaveRequestStatus.PendingC2,
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
