using Hrm.Application.Payroll.Queries;
using Hrm.Domain.Employees.Repositories;
using Hrm.Domain.Identity;
using Hrm.Domain.Identity.Repositories;
using Hrm.Domain.Payroll;
using Hrm.Domain.Payroll.Repositories;
using Jarvis.Domain.Shared.ExceptionHandling;

namespace Hrm.Application.Tests.Payroll;

public sealed class GetPayslipQueryHandlerTests
{
    private static readonly Guid LineId = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd");
    private static readonly Guid EmpId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");

    [Fact]
    public async Task HandleAsync_OwnerNonHr_ReturnsPayslip()
    {
        var handler = new GetPayslipQueryHandler(
            new FakeAccountRepo(["IAM-ROLE-NV"], "MNV-DEV"),
            new FakePayRepo(PayPeriodStatus.Closed, "MNV-DEV"),
            new FakeAuditLogs());

        var dto = await handler.HandleAsync(new GetPayslipQuery("local-nv", LineId));
        Assert.Equal("MNV-DEV", dto.EmployeeCode);
        Assert.Equal("Closed", dto.Status);
    }

    [Fact]
    public async Task HandleAsync_LmOtherEmployee_ThrowsForbidden()
    {
        var handler = new GetPayslipQueryHandler(
            new FakeAccountRepo(["IAM-ROLE-LM", "IAM-ROLE-NV"], "MNV-HO"),
            new FakePayRepo(PayPeriodStatus.Closed, "MNV-DEV"),
            new FakeAuditLogs());

        await Assert.ThrowsAsync<ForbiddenException>(() =>
            handler.HandleAsync(new GetPayslipQuery("local-lm", LineId)));
    }

    [Fact]
    public async Task HandleAsync_Draft_ThrowsNotFound()
    {
        var handler = new GetPayslipQueryHandler(
            new FakeAccountRepo(["IAM-ROLE-NV"], "MNV-DEV"),
            new FakePayRepo(PayPeriodStatus.Draft, "MNV-DEV"),
            new FakeAuditLogs());

        await Assert.ThrowsAsync<NotFoundException>(() =>
            handler.HandleAsync(new GetPayslipQuery("local-nv", LineId)));
    }

    [Fact]
    public async Task HandleAsync_HrAnyEmployee_Returns()
    {
        var handler = new GetPayslipQueryHandler(
            new FakeAccountRepo(["IAM-ROLE-HR", "IAM-ROLE-NV"], "MNV-DEV"),
            new FakePayRepo(PayPeriodStatus.Closed, "MNV-HO"),
            new FakeAuditLogs());

        var dto = await handler.HandleAsync(new GetPayslipQuery("local-dev", LineId));
        Assert.Equal("MNV-HO", dto.EmployeeCode);
    }

    private sealed class FakeAuditLogs : IEmpAuditLogRepository
    {
        public Task AppendAsync(EmpAuditLogEntry entry, CancellationToken cancellationToken = default) =>
            Task.CompletedTask;

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

    private sealed class FakeAccountRepo(string[] roles, string? employeeCode) : IIdentityAccountReadRepository
    {
        public Task<IdentityAccountSnapshot?> FindByIdpSubjectAsync(
            string idpSubject,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IdentityAccountSnapshot?>(new IdentityAccountSnapshot(
                Guid.NewGuid(), idpSubject, idpSubject, null, employeeCode,
                IdentityAccountStatus.Active, roles));

        public Task<IdentityAccountSnapshot?> FindByEmployeeCodeAsync(
            string employeeCode,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IdentityAccountSnapshot?>(null);
    }

    private sealed class FakePayRepo(PayPeriodStatus status, string employeeCode) : IPayPeriodRepository
    {
        public Task<bool> IsClosedAsync(string periodYm, CancellationToken cancellationToken = default) =>
            Task.FromResult(status == PayPeriodStatus.Closed);

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
            Task.FromResult<PayPayslipSnapshot?>(new PayPayslipSnapshot(
                LineId, Guid.NewGuid(), "2028-02", status, EmpId, employeeCode,
                20, 0, 0, 20, 1, 0, 0, 0, 0, 0, 0.1m, 0.05m, 100, 50, 900));

        public Task<IReadOnlyList<PayPayslipSnapshot>> ListClosedPayslipsByEmployeeCodeAsync(
            string code,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<PayPayslipSnapshot>>([]);
    }
}
