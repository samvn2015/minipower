using Hrm.Application.Payroll.Queries;
using Hrm.Domain.Identity;
using Hrm.Domain.Identity.Repositories;
using Hrm.Domain.Payroll;
using Hrm.Domain.Payroll.Repositories;
using Jarvis.Domain.Shared.ExceptionHandling;

namespace Hrm.Application.Tests.Payroll;

public sealed class ListPayrollPeriodsQueryHandlerTests
{
    [Fact]
    public async Task HandleAsync_Lm_Forbidden()
    {
        var handler = new ListPayrollPeriodsQueryHandler(
            new FakeAccountRepo(["IAM-ROLE-LM"]),
            new FakePayRepo(),
            new FakeRegRepo(),
            new FakeCalendar());

        await Assert.ThrowsAsync<ForbiddenException>(() =>
            handler.HandleAsync(new ListPayrollPeriodsQuery("local-lm")));
    }

    [Fact]
    public async Task HandleAsync_Hr_ReturnsList()
    {
        var handler = new ListPayrollPeriodsQueryHandler(
            new FakeAccountRepo(["IAM-ROLE-HR"]),
            new FakePayRepo(),
            new FakeRegRepo(),
            new FakeCalendar());

        var items = await handler.HandleAsync(new ListPayrollPeriodsQuery("local-dev"));
        Assert.Empty(items);
    }

    private sealed class FakeAccountRepo(string[] roles) : IIdentityAccountReadRepository
    {
        public Task<IdentityAccountSnapshot?> FindByIdpSubjectAsync(
            string idpSubject,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IdentityAccountSnapshot?>(new IdentityAccountSnapshot(
                Guid.NewGuid(), idpSubject, idpSubject, null, "MNV-HO",
                IdentityAccountStatus.Active, roles));

        public Task<IdentityAccountSnapshot?> FindByEmployeeCodeAsync(
            string employeeCode,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IdentityAccountSnapshot?>(null);
    }

    private sealed class FakePayRepo : IPayPeriodRepository
    {
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
            CancellationToken cancellationToken = default) =>
            Task.FromResult<PayPeriodSnapshot?>(null);

        public Task MarkClosedAsync(
            string periodYm,
            string closedByIdpSubject,
            CancellationToken cancellationToken = default) =>
            Task.CompletedTask;

        public Task<PayPayslipSnapshot?> FindPayslipByLineIdAsync(
            Guid lineId,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<PayPayslipSnapshot?>(null);

        public Task<IReadOnlyList<PayPayslipSnapshot>> ListClosedPayslipsByEmployeeCodeAsync(
            string employeeCode,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<PayPayslipSnapshot>>([]);
    }

    private sealed class FakeRegRepo : IPayRegulationReadRepository
    {
        public Task<PayRegulationSnapshot?> FindByCodeAsync(
            string code,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<PayRegulationSnapshot?>(new PayRegulationSnapshot(code, code, 22m));
    }

    private sealed class FakeCalendar : IPayWorkdayCalendarRepository
    {
        public Task<decimal> ResolveStandardWorkDaysAsync(
            string periodYm,
            decimal defaultDays,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(defaultDays);

        public Task UpsertAsync(
            string periodYm,
            decimal standardWorkDays,
            CancellationToken cancellationToken = default) =>
            Task.CompletedTask;
    }
}
