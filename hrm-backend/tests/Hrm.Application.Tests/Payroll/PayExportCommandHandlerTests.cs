using Hrm.Application.Payroll.Commands;
using Hrm.Application.Payroll.Dtos;
using Hrm.Domain.Employees;
using Hrm.Domain.Employees.Repositories;
using Hrm.Domain.Identity;
using Hrm.Domain.Identity.Repositories;
using Hrm.Domain.Payroll;
using Hrm.Domain.Payroll.Repositories;
using Jarvis.Domain.Shared.ExceptionHandling;

namespace Hrm.Application.Tests.Payroll;

public sealed class RejectPayLineEditCommandHandlerTests
{
    [Fact]
    public async Task HandleAsync_Hr_AlwaysBadRequest()
    {
        var handler = new RejectPayLineEditCommandHandler(new FakeAccountRepo(["IAM-ROLE-HR"]));
        var ex = await Assert.ThrowsAsync<BadRequestException>(() =>
            handler.HandleAsync(new RejectPayLineEditCommand("local-dev", "2028-04", Guid.NewGuid())));
        Assert.Contains("PAY-FR-008", ex.SystemMessage ?? ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public async Task HandleAsync_Lm_Forbidden()
    {
        var handler = new RejectPayLineEditCommandHandler(new FakeAccountRepo(["IAM-ROLE-LM"]));
        await Assert.ThrowsAsync<ForbiddenException>(() =>
            handler.HandleAsync(new RejectPayLineEditCommand("local-lm", "2028-04", Guid.NewGuid())));
    }

    private sealed class FakeAccountRepo(string[] roles) : IIdentityAccountReadRepository
    {
        public Task<IdentityAccountSnapshot?> FindByIdpSubjectAsync(
            string idpSubject,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IdentityAccountSnapshot?>(new IdentityAccountSnapshot(
                Guid.NewGuid(), idpSubject, idpSubject, null, "MNV-DEV",
                IdentityAccountStatus.Active, roles));

        public Task<IdentityAccountSnapshot?> FindByEmployeeCodeAsync(
            string employeeCode,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IdentityAccountSnapshot?>(null);
    }
}

public sealed class ExportPayrollPeriodCommandHandlerTests
{
    private static readonly Guid EmpId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");
    private static readonly Guid LineId = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd");
    private static readonly Guid PeriodId = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc");

    [Fact]
    public async Task HandleAsync_ClosedWithPdf_ReturnsBase64()
    {
        var outbox = new FakeOutbox();
        var handler = new ExportPayrollPeriodCommandHandler(
            new FakeAccountRepo(["IAM-ROLE-HR"]),
            new FakePayRepo(PayPeriodStatus.Closed),
            new FakeEmployeeRepo("dev@company.local"),
            outbox);

        var result = await handler.HandleAsync(
            new ExportPayrollPeriodCommand("local-dev", "2028-04", true, false, null));

        Assert.Equal(1, result.PdfCount);
        Assert.Equal(0, result.EmailCount);
        Assert.NotNull(result.Items[0].PdfBase64);
        Assert.StartsWith("%PDF", System.Text.Encoding.ASCII.GetString(
            Convert.FromBase64String(result.Items[0].PdfBase64!)));
        Assert.Empty(outbox.Rows);
    }

    [Fact]
    public async Task HandleAsync_CcNotEmpty_BadRequest()
    {
        var handler = new ExportPayrollPeriodCommandHandler(
            new FakeAccountRepo(["IAM-ROLE-HR"]),
            new FakePayRepo(PayPeriodStatus.Closed),
            new FakeEmployeeRepo("dev@company.local"),
            new FakeOutbox());

        await Assert.ThrowsAsync<BadRequestException>(() =>
            handler.HandleAsync(new ExportPayrollPeriodCommand(
                "local-dev", "2028-04", true, true, ["lm@company.local"])));
    }

    [Fact]
    public async Task HandleAsync_Draft_BadRequest()
    {
        var handler = new ExportPayrollPeriodCommandHandler(
            new FakeAccountRepo(["IAM-ROLE-HR"]),
            new FakePayRepo(PayPeriodStatus.Draft),
            new FakeEmployeeRepo("dev@company.local"),
            new FakeOutbox());

        await Assert.ThrowsAsync<BadRequestException>(() =>
            handler.HandleAsync(new ExportPayrollPeriodCommand(
                "local-dev", "2028-04", true, false, null)));
    }

    [Fact]
    public async Task HandleAsync_Email_WritesOutboxToOnly()
    {
        var outbox = new FakeOutbox();
        var handler = new ExportPayrollPeriodCommandHandler(
            new FakeAccountRepo(["IAM-ROLE-HR"]),
            new FakePayRepo(PayPeriodStatus.Closed),
            new FakeEmployeeRepo("dev@company.local"),
            outbox);

        var result = await handler.HandleAsync(
            new ExportPayrollPeriodCommand("local-dev", "2028-04", true, true, null));

        Assert.Equal(1, result.EmailCount);
        Assert.Single(outbox.Rows);
        Assert.Equal("dev@company.local", outbox.Rows[0].ToAddress);
        Assert.Null(outbox.Rows[0].CcAddress);
    }

    [Fact]
    public async Task HandleAsync_Lm_Forbidden()
    {
        var handler = new ExportPayrollPeriodCommandHandler(
            new FakeAccountRepo(["IAM-ROLE-LM"]),
            new FakePayRepo(PayPeriodStatus.Closed),
            new FakeEmployeeRepo("dev@company.local"),
            new FakeOutbox());

        await Assert.ThrowsAsync<ForbiddenException>(() =>
            handler.HandleAsync(new ExportPayrollPeriodCommand(
                "local-lm", "2028-04", true, false, null)));
    }

    private sealed class FakeAccountRepo(string[] roles) : IIdentityAccountReadRepository
    {
        public Task<IdentityAccountSnapshot?> FindByIdpSubjectAsync(
            string idpSubject,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IdentityAccountSnapshot?>(new IdentityAccountSnapshot(
                Guid.NewGuid(), idpSubject, idpSubject, null, "MNV-DEV",
                IdentityAccountStatus.Active, roles));

        public Task<IdentityAccountSnapshot?> FindByEmployeeCodeAsync(
            string employeeCode,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IdentityAccountSnapshot?>(null);
    }

    private sealed class FakeEmployeeRepo(string? email) : IEmployeeReadRepository
    {
        public Task<IReadOnlyList<EmployeeSnapshot>> ListAsync(CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<EmployeeSnapshot>>([]);

        public Task<EmployeeSnapshot?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
            Task.FromResult<EmployeeSnapshot?>(new EmployeeSnapshot(
                EmpId, "MNV-DEV", "Dev", null, email, null, null, null, null, null,
                null, null, EmployeeStatus.Active));

        public Task<EmployeeSnapshot?> FindByEmployeeCodeAsync(
            string employeeCode,
            CancellationToken cancellationToken = default) =>
            FindByIdAsync(EmpId, cancellationToken);

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

    private sealed class FakePayRepo(PayPeriodStatus status) : IPayPeriodRepository
    {
        public Task<bool> IsClosedAsync(string periodYm, CancellationToken cancellationToken = default) =>
            Task.FromResult(status == PayPeriodStatus.Closed);

        public Task<PayPeriodSnapshot?> FindByYmAsync(
            string periodYm,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<PayPeriodSnapshot?>(new PayPeriodSnapshot(
                PeriodId,
                periodYm,
                status,
                1,
                [
                    new PayLineSnapshot(
                        LineId, EmpId, "MNV-DEV", 20, 0, 0, 20, 1m,
                        0, 0, 0, 0, 0, 0.1m, 0.05m, 0, 0, 0)
                ]));

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

    private sealed class FakeOutbox : IPayExportOutboxRepository
    {
        public List<PayExportOutboxCreateModel> Rows { get; } = [];

        public Task AddManyAsync(
            IReadOnlyList<PayExportOutboxCreateModel> rows,
            CancellationToken cancellationToken = default)
        {
            Rows.AddRange(rows);
            return Task.CompletedTask;
        }
    }
}

public sealed class PayExportRecipientGuardTests
{
    [Fact]
    public void EnsureNoCc_WithAddress_Throws()
    {
        Assert.Throws<InvalidOperationException>(() =>
            PayExportRecipientGuard.EnsureNoCc(["a@b.c"]));
    }

    [Fact]
    public void EnsureToMatchesEmployee_Mismatch_Throws()
    {
        Assert.Throws<InvalidOperationException>(() =>
            PayExportRecipientGuard.EnsureToMatchesEmployee("a@b.c", "x@y.z"));
    }
}
