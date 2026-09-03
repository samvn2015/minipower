using Hrm.Application.Payroll.Commands;
using Hrm.Domain.Identity;
using Hrm.Domain.Identity.Repositories;
using Hrm.Domain.Payroll;
using Hrm.Domain.Payroll.Repositories;
using Hrm.Domain.Timekeeping;
using Hrm.Domain.Timekeeping.Repositories;
using Jarvis.Domain.Shared.ExceptionHandling;

namespace Hrm.Application.Tests.Payroll;

public sealed class RunPayrollPeriodCommandHandlerTests
{
    private static readonly Guid EmpId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");
    private static readonly Guid TimPeriodId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");

    [Fact]
    public async Task HandleAsync_TimClosed_CreatesDraftWithNTinh()
    {
        var tim = new FakeTimRepo(TimesheetPeriodStatus.Closed, workDays: 22, unpaid: 2, paid: 2);
        var pay = new FakePayRepo();
        var handler = new RunPayrollPeriodCommandHandler(
            new FakeAccountRepo(["IAM-ROLE-HR"]),
            tim,
            pay);

        var result = await handler.HandleAsync(new RunPayrollPeriodCommand("local-dev", "2027-07"));

        Assert.Equal("Draft", result.Status);
        Assert.Equal(1, result.LineCount);
        Assert.NotNull(pay.LastLines);
        Assert.Equal(20m, pay.LastLines![0].NTinh); // 22 - 2
        Assert.Equal(2m, pay.LastLines[0].LeaveDaysPaid); // audit only — not added to N_tính
    }

    [Fact]
    public async Task HandleAsync_TimNotClosed_ThrowsBadRequest()
    {
        var handler = new RunPayrollPeriodCommandHandler(
            new FakeAccountRepo(["IAM-ROLE-HR"]),
            new FakeTimRepo(TimesheetPeriodStatus.Draft, 20, 0, 0),
            new FakePayRepo());

        await Assert.ThrowsAsync<BadRequestException>(() =>
            handler.HandleAsync(new RunPayrollPeriodCommand("local-dev", "2027-07")));
    }

    [Fact]
    public async Task HandleAsync_LmForbidden()
    {
        var handler = new RunPayrollPeriodCommandHandler(
            new FakeAccountRepo(["IAM-ROLE-LM"]),
            new FakeTimRepo(TimesheetPeriodStatus.Closed, 20, 0, 0),
            new FakePayRepo());

        await Assert.ThrowsAsync<ForbiddenException>(() =>
            handler.HandleAsync(new RunPayrollPeriodCommand("local-lm", "2027-07")));
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

    private sealed class FakeTimRepo(
        TimesheetPeriodStatus status,
        decimal workDays,
        decimal unpaid,
        decimal paid) : ITimesheetImportRepository
    {
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
            Task.FromResult<TimesheetPeriodSnapshot?>(new TimesheetPeriodSnapshot(
                TimPeriodId,
                periodYm,
                status,
                null,
                1,
                [
                    new TimesheetLineSnapshot(
                        Guid.NewGuid(), EmpId, "MNV-DEV", workDays, 1, 0, 0, 0, paid, unpaid, 0)
                ]));

        public Task<IReadOnlyList<TimesheetPeriodSnapshot>> ListPeriodsAsync(
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<TimesheetPeriodSnapshot>>([]);

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
            CancellationToken cancellationToken = default) =>
            Task.FromResult<TimesheetPeriodSnapshot?>(null);
    }

    private sealed class FakePayRepo : IPayPeriodRepository
    {
        public IReadOnlyList<PayLineCreateModel>? LastLines { get; private set; }

        public Task<bool> IsClosedAsync(string periodYm, CancellationToken cancellationToken = default) =>
            Task.FromResult(false);

        public Task<PayPeriodSnapshot?> FindByYmAsync(
            string periodYm,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<PayPeriodSnapshot?>(null);

        public Task<IReadOnlyList<PayPeriodSnapshot>> ListAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<PayPeriodSnapshot>>([]);

        public Task<PayPeriodSnapshot?> RunDraftAsync(
            string periodYm,
            string ranByIdpSubject,
            IReadOnlyList<PayLineCreateModel> lines,
            CancellationToken cancellationToken = default)
        {
            LastLines = lines;
            return Task.FromResult<PayPeriodSnapshot?>(new PayPeriodSnapshot(
                Guid.NewGuid(),
                periodYm,
                PayPeriodStatus.Draft,
                lines.Count,
                lines.Select(l => new PayLineSnapshot(
                    Guid.NewGuid(),
                    l.EmployeeId,
                    l.EmployeeCode,
                    l.WorkDays,
                    l.LeaveDaysUnpaid,
                    l.LeaveDaysPaid,
                    l.NTinh,
                    l.Ot15,
                    l.Ot20,
                    l.Ot30)).ToList()));
        }

        public Task MarkClosedAsync(
            string periodYm,
            string closedByIdpSubject,
            CancellationToken cancellationToken = default) =>
            Task.CompletedTask;
    }
}
