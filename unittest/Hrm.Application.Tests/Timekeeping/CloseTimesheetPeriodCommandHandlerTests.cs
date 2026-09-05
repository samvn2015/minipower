using Hrm.Application.Timekeeping.Commands;
using Hrm.Domain.Employees;
using Hrm.Domain.Employees.Repositories;
using Hrm.Domain.Identity;
using Hrm.Domain.Identity.Repositories;
using Hrm.Domain.Leave;
using Hrm.Domain.Leave.Repositories;
using Hrm.Domain.Timekeeping;
using Hrm.Domain.Timekeeping.Repositories;
using Jarvis.Domain.Shared.ExceptionHandling;

namespace Hrm.Application.Tests.Timekeeping;

public sealed class CloseTimesheetPeriodCommandHandlerTests
{
    private static readonly Guid PeriodId = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc");
    private static readonly Guid EmployeeId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");

    [Fact]
    public async Task HandleAsync_DraftClean_Closes()
    {
        var imports = new FakeImportRepo(
            new TimesheetPeriodSnapshot(
                PeriodId,
                "2026-11",
                TimesheetPeriodStatus.Draft,
                null,
                1,
                [Line(workDays: 20, otUnclassified: 0)]));

        var audit = new FakeAudit();
        var handler = new CloseTimesheetPeriodCommandHandler(
            new FakeAccountRepo(["IAM-ROLE-HR"]),
            imports,
            new FakeLeaveRepo([]),
            audit);

        var result = await handler.HandleAsync(new CloseTimesheetPeriodCommand("local-dev", "2026-11"));

        Assert.Equal("Closed", result.Status);
        Assert.Equal("2026-11", result.PeriodYm);
        Assert.True(imports.CloseCalled);
        Assert.Equal(0m, result.TotalLeaveDaysPaid);
        Assert.Contains(audit.Entries, e => e.Action == EmpAuditActions.TimesheetPeriodClosed);
    }

    [Fact]
    public async Task HandleAsync_ApprovedPaidLeave_MergedIntoWorkDays()
    {
        var imports = new FakeImportRepo(
            new TimesheetPeriodSnapshot(
                PeriodId,
                "2026-11",
                TimesheetPeriodStatus.Draft,
                null,
                1,
                [Line(workDays: 20, otUnclassified: 0)]));

        var leaves = new List<ApprovedLeaveForTimesheetSnapshot>
        {
            new(
                Guid.NewGuid(),
                EmployeeId,
                "LEV-ANNUAL",
                DeductsAnnualBalance: true,
                new DateOnly(2026, 11, 10),
                new DateOnly(2026, 11, 11),
                2m)
        };

        var handler = new CloseTimesheetPeriodCommandHandler(
            new FakeAccountRepo(["IAM-ROLE-HR"]),
            imports,
            new FakeLeaveRepo(leaves),
            new FakeAudit());

        var result = await handler.HandleAsync(new CloseTimesheetPeriodCommand("local-dev", "2026-11"));

        Assert.Equal("Closed", result.Status);
        Assert.Equal(2m, result.TotalLeaveDaysPaid);
        Assert.Equal(22m, imports.LastClosed!.Lines[0].WorkDays);
        Assert.Equal(2m, imports.LastClosed.Lines[0].LeaveDaysPaid);
    }

    [Fact]
    public async Task HandleAsync_PendingLeave_NotMerged()
    {
        // Leave repo only returns Approved — PendingC1 never appears.
        var imports = new FakeImportRepo(
            new TimesheetPeriodSnapshot(
                PeriodId,
                "2026-11",
                TimesheetPeriodStatus.Draft,
                null,
                1,
                [Line(workDays: 20, otUnclassified: 0)]));

        var handler = new CloseTimesheetPeriodCommandHandler(
            new FakeAccountRepo(["IAM-ROLE-HR"]),
            imports,
            new FakeLeaveRepo([]),
            new FakeAudit());

        var result = await handler.HandleAsync(new CloseTimesheetPeriodCommand("local-dev", "2026-11"));

        Assert.Equal(0m, result.TotalLeaveDaysPaid);
        Assert.Equal(20m, imports.LastClosed!.Lines[0].WorkDays);
    }

    [Fact]
    public async Task HandleAsync_OtUnclassified_ThrowsBadRequest()
    {
        var imports = new FakeImportRepo(
            new TimesheetPeriodSnapshot(
                PeriodId,
                "2026-11",
                TimesheetPeriodStatus.Draft,
                null,
                1,
                [Line(workDays: 20, otUnclassified: 3)]));

        var handler = new CloseTimesheetPeriodCommandHandler(
            new FakeAccountRepo(["IAM-ROLE-HR"]),
            imports,
            new FakeLeaveRepo([]),
            new FakeAudit());

        await Assert.ThrowsAsync<BadRequestException>(() =>
            handler.HandleAsync(new CloseTimesheetPeriodCommand("local-dev", "2026-11")));
        Assert.False(imports.CloseCalled);
    }

    [Fact]
    public async Task HandleAsync_LmCannotClose_ThrowsForbidden()
    {
        var imports = new FakeImportRepo(
            new TimesheetPeriodSnapshot(
                PeriodId,
                "2026-11",
                TimesheetPeriodStatus.Draft,
                null,
                0,
                []));

        var handler = new CloseTimesheetPeriodCommandHandler(
            new FakeAccountRepo(["IAM-ROLE-LM"]),
            imports,
            new FakeLeaveRepo([]),
            new FakeAudit());

        await Assert.ThrowsAsync<ForbiddenException>(() =>
            handler.HandleAsync(new CloseTimesheetPeriodCommand("local-lm", "2026-11")));
    }

    private static TimesheetLineSnapshot Line(decimal workDays, decimal otUnclassified) =>
        new(Guid.NewGuid(), EmployeeId, "MNV-DEV", workDays, 0, 0, 0, otUnclassified, 0, 0, 0);


    private sealed class FakeAudit : IEmpAuditLogRepository
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

    private sealed class FakeAccountRepo(string[] roles) : IIdentityAccountReadRepository
    {
        public Task<IdentityAccountSnapshot?> FindByIdpSubjectAsync(
            string idpSubject,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IdentityAccountSnapshot?>(new IdentityAccountSnapshot(
                Guid.NewGuid(), idpSubject, idpSubject, null, null, IdentityAccountStatus.Active, roles));

        public Task<IdentityAccountSnapshot?> FindByEmployeeCodeAsync(
            string employeeCode,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IdentityAccountSnapshot?>(null);
    }

    private sealed class FakeLeaveRepo(IReadOnlyList<ApprovedLeaveForTimesheetSnapshot> leaves)
        : ILeaveRequestRepository
    {
        public Task<Guid> CreateAsync(LeaveRequestCreateModel model, CancellationToken cancellationToken = default)
            => Task.FromResult(Guid.NewGuid());

        public Task<LeaveRequestSnapshot?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => Task.FromResult<LeaveRequestSnapshot?>(null);

        public Task<IReadOnlyList<LeaveRequestSnapshot>> ListByEmployeeIdAsync(
            Guid employeeId,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<LeaveRequestSnapshot>>([]);

        public Task<IReadOnlyList<LeaveRequestPendingC1Snapshot>> ListPendingC1ByLineManagerIdAsync(
            Guid lineManagerEmployeeId,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<LeaveRequestPendingC1Snapshot>>([]);

        public Task<bool> ApproveC1Async(Guid id, string reviewedByIdpSubject, CancellationToken cancellationToken = default)
            => Task.FromResult(true);

        public Task<bool> RejectC1Async(
            Guid id, string reviewedByIdpSubject, string? reviewNote, CancellationToken cancellationToken = default)
            => Task.FromResult(true);

        public Task<IReadOnlyList<LeaveRequestPendingC1Snapshot>> ListPendingC2Async(
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<LeaveRequestPendingC1Snapshot>>([]);

        public Task<bool> ApproveC2Async(
            Guid id, string reviewedByIdpSubject, bool deductsAnnualBalance, CancellationToken cancellationToken = default)
            => Task.FromResult(true);

        public Task<bool> RejectC2Async(
            Guid id, string reviewedByIdpSubject, string? reviewNote, CancellationToken cancellationToken = default)
            => Task.FromResult(true);

        public Task<bool> HasOpenOverlapAsync(
            Guid employeeId, DateOnly fromDate, DateOnly toDate, LeaveDayPart dayPart,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(false);

        public Task<bool> CancelByEmployeeAsync(Guid id, Guid employeeId, CancellationToken cancellationToken = default)
            => Task.FromResult(true);

        public Task<IReadOnlyList<ApprovedLeaveForTimesheetSnapshot>> ListApprovedOverlappingPeriodAsync(
            string periodYm,
            IReadOnlyList<Guid> employeeIds,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(leaves);
    }

    private sealed class FakeImportRepo(TimesheetPeriodSnapshot period) : ITimesheetImportRepository
    {
        public bool CloseCalled { get; private set; }
        public TimesheetPeriodSnapshot? LastClosed { get; private set; }

        public Task<Guid> CreatePreviewAsync(
            TimesheetImportBatchCreateModel model,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(Guid.NewGuid());

        public Task<TimesheetImportBatchSnapshot?> FindBatchByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<TimesheetImportBatchSnapshot?>(null);

        public Task<TimesheetPeriodSnapshot?> FindPeriodByYmAsync(
            string periodYm,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<TimesheetPeriodSnapshot?>(
                periodYm == period.PeriodYm ? period : null);

        public Task<IReadOnlyList<TimesheetPeriodSnapshot>> ListPeriodsAsync(
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<TimesheetPeriodSnapshot>>([period]);

        public Task<TimesheetPeriodSnapshot?> CommitAsync(
            Guid batchId,
            string committedByIdpSubject,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<TimesheetPeriodSnapshot?>(null);

        public Task<TimesheetPeriodSnapshot?> ClosePeriodAsync(
            string periodYm,
            string closedByIdpSubject,
            IReadOnlyList<TimesheetLeaveMergeLine> leaveMerge,
            CancellationToken cancellationToken = default)
        {
            CloseCalled = true;
            var mergeMap = leaveMerge.ToDictionary(x => x.EmployeeId);
            var lines = period.Lines.Select(l =>
            {
                mergeMap.TryGetValue(l.EmployeeId, out var m);
                var paid = m?.LeaveDaysPaid ?? 0;
                var unpaid = m?.LeaveDaysUnpaid ?? 0;
                var other = m?.LeaveDaysOther ?? 0;
                return l with
                {
                    WorkDays = l.WorkDays + paid,
                    LeaveDaysPaid = paid,
                    LeaveDaysUnpaid = unpaid,
                    LeaveDaysOther = other
                };
            }).ToList();

            LastClosed = period with
            {
                Status = TimesheetPeriodStatus.Closed,
                Lines = lines,
                LineCount = lines.Count
            };
            return Task.FromResult<TimesheetPeriodSnapshot?>(LastClosed);
        }

        public Task<TimesheetPeriodSnapshot?> UnlockPeriodAsync(
            string periodYm,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<TimesheetPeriodSnapshot?>(null);
    }
}
