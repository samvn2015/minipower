using Hrm.Application.Timekeeping.Commands;
using Hrm.Domain.Identity;
using Hrm.Domain.Identity.Repositories;
using Hrm.Domain.Payroll.Repositories;
using Hrm.Domain.Timekeeping;
using Hrm.Domain.Timekeeping.Repositories;
using Jarvis.Domain.Shared.ExceptionHandling;

namespace Hrm.Application.Tests.Timekeeping;

public sealed class UnlockTimesheetPeriodCommandHandlerTests
{
    private static readonly Guid PeriodId = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd");
    private static readonly Guid EmployeeId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");

    [Fact]
    public async Task HandleAsync_Closed_UnlocksToDraft()
    {
        var imports = new FakeImportRepo(
            new TimesheetPeriodSnapshot(
                PeriodId,
                "2027-04",
                TimesheetPeriodStatus.Closed,
                null,
                1,
                [
                    new TimesheetLineSnapshot(
                        Guid.NewGuid(), EmployeeId, "MNV-DEV", 22, 0, 0, 0, 0, 2, 0, 0)
                ]));

        var handler = new UnlockTimesheetPeriodCommandHandler(
            new FakeAccountRepo(["IAM-ROLE-HR"]),
            imports,
            new FakePayGate(closed: false));

        var result = await handler.HandleAsync(new UnlockTimesheetPeriodCommand("local-dev", "2027-04"));

        Assert.Equal("Draft", result.Status);
        Assert.True(imports.UnlockCalled);
    }

    [Fact]
    public async Task HandleAsync_PayClosed_ThrowsConflict()
    {
        var imports = new FakeImportRepo(
            new TimesheetPeriodSnapshot(
                PeriodId,
                "2027-04",
                TimesheetPeriodStatus.Closed,
                null,
                0,
                []));

        var handler = new UnlockTimesheetPeriodCommandHandler(
            new FakeAccountRepo(["IAM-ROLE-HR"]),
            imports,
            new FakePayGate(closed: true));

        await Assert.ThrowsAsync<ConflictException>(() =>
            handler.HandleAsync(new UnlockTimesheetPeriodCommand("local-dev", "2027-04")));
        Assert.False(imports.UnlockCalled);
    }

    [Fact]
    public async Task HandleAsync_LmCannotUnlock_ThrowsForbidden()
    {
        var imports = new FakeImportRepo(
            new TimesheetPeriodSnapshot(
                PeriodId,
                "2027-04",
                TimesheetPeriodStatus.Closed,
                null,
                0,
                []));

        var handler = new UnlockTimesheetPeriodCommandHandler(
            new FakeAccountRepo(["IAM-ROLE-LM"]),
            imports,
            new FakePayGate(closed: false));

        await Assert.ThrowsAsync<ForbiddenException>(() =>
            handler.HandleAsync(new UnlockTimesheetPeriodCommand("local-lm", "2027-04")));
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

    private sealed class FakePayGate(bool closed) : IPayPeriodGate
    {
        public Task<bool> IsClosedAsync(string periodYm, CancellationToken cancellationToken = default) =>
            Task.FromResult(closed);
    }

    private sealed class FakeImportRepo(TimesheetPeriodSnapshot period) : ITimesheetImportRepository
    {
        public bool UnlockCalled { get; private set; }

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
            CancellationToken cancellationToken = default) =>
            Task.FromResult<TimesheetPeriodSnapshot?>(null);

        public Task<TimesheetPeriodSnapshot?> UnlockPeriodAsync(
            string periodYm,
            CancellationToken cancellationToken = default)
        {
            UnlockCalled = true;
            return Task.FromResult<TimesheetPeriodSnapshot?>(period with
            {
                Status = TimesheetPeriodStatus.Draft
            });
        }
    }
}
