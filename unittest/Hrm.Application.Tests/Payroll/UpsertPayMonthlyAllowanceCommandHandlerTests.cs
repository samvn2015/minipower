using Hrm.Application.Payroll.Commands;
using Hrm.Domain.Employees;
using Hrm.Domain.Employees.Repositories;
using Hrm.Domain.Identity;
using Hrm.Domain.Identity.Repositories;
using Hrm.Domain.Payroll;
using Hrm.Domain.Payroll.Repositories;
using Jarvis.Domain.Shared.ExceptionHandling;

namespace Hrm.Application.Tests.Payroll;

public sealed class UpsertPayMonthlyAllowanceCommandHandlerTests
{
    private static readonly Guid EmpId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");

    [Fact]
    public async Task HandleAsync_UnknownCode_ThrowsBadRequest()
    {
        var handler = new UpsertPayMonthlyAllowanceCommandHandler(
            new FakeAccountRepo(["IAM-ROLE-HR"]),
            new FakeEmployeeRepo(),
            new FakeAllowance(codeActive: false),
            new FakePayGate(closed: false));

        await Assert.ThrowsAsync<BadRequestException>(() =>
            handler.HandleAsync(new UpsertPayMonthlyAllowanceCommand(
                "local-dev", "2027-12", "MNV-DEV", "PC-LA", 1000)));
    }

    [Fact]
    public async Task HandleAsync_KnownCode_Upserts()
    {
        var allowance = new FakeAllowance(codeActive: true);
        var handler = new UpsertPayMonthlyAllowanceCommandHandler(
            new FakeAccountRepo(["IAM-ROLE-HR"]),
            new FakeEmployeeRepo(),
            allowance,
            new FakePayGate(closed: false));

        var result = await handler.HandleAsync(new UpsertPayMonthlyAllowanceCommand(
            "local-dev", "2027-12", "MNV-DEV", "PC-XANG", 200_000m));

        Assert.Equal("PC-XANG", result.Code);
        Assert.Equal(200_000m, result.Amount);
        Assert.True(allowance.Upserted);
    }

    [Fact]
    public async Task HandleAsync_LmForbidden()
    {
        var handler = new UpsertPayMonthlyAllowanceCommandHandler(
            new FakeAccountRepo(["IAM-ROLE-LM"]),
            new FakeEmployeeRepo(),
            new FakeAllowance(true),
            new FakePayGate(false));

        await Assert.ThrowsAsync<ForbiddenException>(() =>
            handler.HandleAsync(new UpsertPayMonthlyAllowanceCommand(
                "local-lm", "2027-12", "MNV-DEV", "PC-XANG", 1)));
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

    private sealed class FakeEmployeeRepo : IEmployeeReadRepository
    {
        public Task<IReadOnlyList<EmployeeSnapshot>> ListAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<EmployeeSnapshot>>([]);

        public Task<EmployeeSnapshot?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
            FindByEmployeeCodeAsync("MNV-DEV", cancellationToken);

        public Task<EmployeeSnapshot?> FindByEmployeeCodeAsync(
            string employeeCode,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<EmployeeSnapshot?>(new EmployeeSnapshot(
                EmpId, "MNV-DEV", "Dev", null, null, null, null, null, null, null,
                null, null, EmployeeStatus.Active));

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

    private sealed class FakePayGate(bool closed) : IPayPeriodRepository
    {
        public Task<bool> IsClosedAsync(string periodYm, CancellationToken cancellationToken = default) =>
            Task.FromResult(closed);

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

    private sealed class FakeAllowance(bool codeActive) : IPayAllowanceRepository
    {
        public bool Upserted { get; private set; }

        public Task<IReadOnlyList<PayAllowanceCatalogSnapshot>> ListCatalogAsync(
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<PayAllowanceCatalogSnapshot>>([]);

        public Task<bool> IsActiveCodeAsync(string code, CancellationToken cancellationToken = default) =>
            Task.FromResult(codeActive);

        public Task<decimal> SumContractAsync(Guid employeeId, CancellationToken cancellationToken = default) =>
            Task.FromResult(0m);

        public Task<decimal> SumMonthlyAsync(
            string periodYm,
            Guid employeeId,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(0m);

        public Task<decimal> SumMealTaxExemptAsync(
            string periodYm,
            Guid employeeId,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(0m);

        public Task<decimal> SumAdvanceAsync(
            string periodYm,
            Guid employeeId,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(0m);

        public Task<IReadOnlyList<string>> ListUnknownMonthlyCodesAsync(
            string periodYm,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<string>>([]);

        public Task<IReadOnlyList<PayMonthlyAllowanceSnapshot>> ListMonthlyByYmAsync(
            string periodYm,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<PayMonthlyAllowanceSnapshot>>([]);

        public Task UpsertMonthlyAsync(
            string periodYm,
            Guid employeeId,
            string employeeCode,
            string code,
            decimal amount,
            CancellationToken cancellationToken = default)
        {
            Upserted = true;
            return Task.CompletedTask;
        }
    }
}
