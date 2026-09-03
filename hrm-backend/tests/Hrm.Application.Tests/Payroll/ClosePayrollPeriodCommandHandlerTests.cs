using Hrm.Application.Payroll.Commands;
using Hrm.Domain.Identity;
using Hrm.Domain.Identity.Repositories;
using Hrm.Domain.Payroll;
using Hrm.Domain.Payroll.Repositories;
using Jarvis.Domain.Shared.ExceptionHandling;

namespace Hrm.Application.Tests.Payroll;

public sealed class ClosePayrollPeriodCommandHandlerTests
{
    private static readonly Guid EmpId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");

    [Fact]
    public async Task HandleAsync_NTinhOverCap_ThrowsBadRequest()
    {
        var pay = new FakePayRepo(
            nTinh: 23,
            closed: false);
        var handler = new ClosePayrollPeriodCommandHandler(
            new FakeAccountRepo(["IAM-ROLE-HR"]),
            pay,
            new FakeRegRepo(22m),
            new FakeCalendar(21m));

        await Assert.ThrowsAsync<BadRequestException>(() =>
            handler.HandleAsync(new ClosePayrollPeriodCommand("local-dev", "2027-10")));
        Assert.False(pay.CloseCalled);
    }

    [Fact]
    public async Task HandleAsync_NTinhWithinCap_Closes()
    {
        var pay = new FakePayRepo(nTinh: 20, closed: false);
        var handler = new ClosePayrollPeriodCommandHandler(
            new FakeAccountRepo(["IAM-ROLE-HR"]),
            pay,
            new FakeRegRepo(22m),
            new FakeCalendar(22m));

        var result = await handler.HandleAsync(new ClosePayrollPeriodCommand("local-dev", "2027-10"));
        Assert.Equal("Closed", result.Status);
        Assert.True(pay.CloseCalled);
    }

    [Fact]
    public async Task HandleAsync_NoLines_SkipsCapAndCloses()
    {
        var pay = new FakePayRepo(nTinh: 99, closed: false, lineCount: 0);
        var handler = new ClosePayrollPeriodCommandHandler(
            new FakeAccountRepo(["IAM-ROLE-HR"]),
            pay,
            new FakeRegRepo(22m),
            new FakeCalendar(21m));

        var result = await handler.HandleAsync(new ClosePayrollPeriodCommand("local-dev", "2027-10"));
        Assert.Equal("Closed", result.Status);
        Assert.True(pay.CloseCalled);
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

    private sealed class FakeRegRepo(decimal days) : IPayRegulationReadRepository
    {
        public Task<PayRegulationSnapshot?> FindByCodeAsync(
            string code,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<PayRegulationSnapshot?>(new PayRegulationSnapshot(code, "std", days));
    }

    private sealed class FakeCalendar(decimal days) : IPayWorkdayCalendarRepository
    {
        public Task<decimal> ResolveStandardWorkDaysAsync(
            string periodYm,
            decimal defaultStandardWorkDays,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(days);

        public Task UpsertAsync(
            string periodYm,
            decimal standardWorkDays,
            CancellationToken cancellationToken = default) =>
            Task.CompletedTask;
    }

    private sealed class FakePayRepo(decimal nTinh, bool closed, int lineCount = 1) : IPayPeriodRepository
    {
        private bool _closed = closed;

        public bool CloseCalled { get; private set; }

        public Task<bool> IsClosedAsync(string periodYm, CancellationToken cancellationToken = default) =>
            Task.FromResult(_closed);

        public Task<PayPeriodSnapshot?> FindByYmAsync(
            string periodYm,
            CancellationToken cancellationToken = default)
        {
            var lines = lineCount > 0
                ? new PayLineSnapshot[]
                {
                    new(Guid.NewGuid(), EmpId, "MNV-DEV", nTinh, 0, 0, nTinh, 1, 0, 0, 0)
                }
                : [];
            return Task.FromResult<PayPeriodSnapshot?>(new PayPeriodSnapshot(
                Guid.NewGuid(),
                periodYm,
                _closed ? PayPeriodStatus.Closed : PayPeriodStatus.Draft,
                lineCount,
                lines));
        }

        public Task<IReadOnlyList<PayPeriodSnapshot>> ListAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<PayPeriodSnapshot>>([]);

        public Task<PayPeriodSnapshot?> RunDraftAsync(
            string periodYm,
            string ranByIdpSubject,
            IReadOnlyList<PayLineCreateModel> lines,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<PayPeriodSnapshot?>(null);

        public Task MarkClosedAsync(
            string periodYm,
            string closedByIdpSubject,
            CancellationToken cancellationToken = default)
        {
            CloseCalled = true;
            _closed = true;
            return Task.CompletedTask;
        }
    }
}
