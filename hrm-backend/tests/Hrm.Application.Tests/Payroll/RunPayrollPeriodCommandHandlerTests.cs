using Hrm.Application.Payroll.Commands;
using Hrm.Domain.Employees;
using Hrm.Domain.Employees.Repositories;
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
        var pay = new FakePayRepo();
        var handler = new RunPayrollPeriodCommandHandler(
            new FakeAccountRepo(["IAM-ROLE-HR"]),
            new FakeTimRepo(TimesheetPeriodStatus.Closed, workDays: 22, unpaid: 2, paid: 2, ot15: 1),
            pay,
            new FakeEmployeeRepo(probation: false),
            new FakeRegRepo(),
            new FakeAllowance(),
            new FakeSalary());

        var result = await handler.HandleAsync(new RunPayrollPeriodCommand("local-dev", "2027-07"));

        Assert.Equal("Draft", result.Status);
        Assert.Equal(1, result.LineCount);
        Assert.NotNull(pay.LastLines);
        Assert.Equal(20m, pay.LastLines![0].NTinh);
        Assert.Equal(1.00m, pay.LastLines[0].TimeWageFactor);
        Assert.Equal(1m, pay.LastLines[0].Ot15); // OT từ TIM (FR-004)
        Assert.Equal(0m, pay.LastLines[0].ContractAllowance);
        Assert.Equal(0m, pay.LastLines[0].MonthlyAllowance);
    }

    [Fact]
    public async Task HandleAsync_SumsContractAndMonthlyAllowances()
    {
        var pay = new FakePayRepo();
        var handler = new RunPayrollPeriodCommandHandler(
            new FakeAccountRepo(["IAM-ROLE-HR"]),
            new FakeTimRepo(TimesheetPeriodStatus.Closed, 20, 0, 0, 0),
            pay,
            new FakeEmployeeRepo(probation: false),
            new FakeRegRepo(),
            new FakeAllowance(contract: 730_000m, monthly: 200_000m),
            new FakeSalary());

        await handler.HandleAsync(new RunPayrollPeriodCommand("local-dev", "2027-12"));

        Assert.Equal(730_000m, pay.LastLines![0].ContractAllowance);
        Assert.Equal(200_000m, pay.LastLines[0].MonthlyAllowance);
    }

    [Fact]
    public async Task HandleAsync_UsesMasterBhTncnRatesNotHardcoded()
    {
        var pay = new FakePayRepo();
        var handler = new RunPayrollPeriodCommandHandler(
            new FakeAccountRepo(["IAM-ROLE-HR"]),
            new FakeTimRepo(TimesheetPeriodStatus.Closed, 20, 0, 0, 0),
            pay,
            new FakeEmployeeRepo(probation: false),
            new FakeRegRepo(bh: 0.10m, tncn: 0.05m),
            new FakeAllowance(),
            new FakeSalary(10_000_000m));

        await handler.HandleAsync(new RunPayrollPeriodCommand("local-dev", "2028-01"));

        var line = pay.LastLines![0];
        Assert.Equal(0.10m, line.BhRate);
        Assert.Equal(0.05m, line.TncnRate);
        var expected = PayrollStatutoryCalculator.Compute(10_000_000m, 1m, 0m, 0m, 0.10m, 0.05m);
        Assert.Equal(expected.BhAmount, line.BhAmount);
        Assert.Equal(expected.TncnAmount, line.TncnAmount);
        Assert.Equal(expected.NetPay, line.NetPay);
    }

    [Fact]
    public async Task HandleAsync_ProbationContract_AppliesMasterFactor()
    {
        var pay = new FakePayRepo();
        var handler = new RunPayrollPeriodCommandHandler(
            new FakeAccountRepo(["IAM-ROLE-HR"]),
            new FakeTimRepo(TimesheetPeriodStatus.Closed, 20, 0, 0, 0),
            pay,
            new FakeEmployeeRepo(probation: true),
            new FakeRegRepo(),
            new FakeAllowance(),
            new FakeSalary());

        await handler.HandleAsync(new RunPayrollPeriodCommand("local-dev", "2027-07"));

        Assert.Equal(0.85m, pay.LastLines![0].TimeWageFactor);
    }

    [Fact]
    public async Task HandleAsync_TimNotClosed_ThrowsBadRequest()
    {
        var handler = new RunPayrollPeriodCommandHandler(
            new FakeAccountRepo(["IAM-ROLE-HR"]),
            new FakeTimRepo(TimesheetPeriodStatus.Draft, 20, 0, 0, 0),
            new FakePayRepo(),
            new FakeEmployeeRepo(false),
            new FakeRegRepo(),
            new FakeAllowance(),
            new FakeSalary());

        await Assert.ThrowsAsync<BadRequestException>(() =>
            handler.HandleAsync(new RunPayrollPeriodCommand("local-dev", "2027-07")));
    }

    [Fact]
    public async Task HandleAsync_LmForbidden()
    {
        var handler = new RunPayrollPeriodCommandHandler(
            new FakeAccountRepo(["IAM-ROLE-LM"]),
            new FakeTimRepo(TimesheetPeriodStatus.Closed, 20, 0, 0, 0),
            new FakePayRepo(),
            new FakeEmployeeRepo(false),
            new FakeRegRepo(),
            new FakeAllowance(),
            new FakeSalary());

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

    private sealed class FakeRegRepo(
        decimal probation = 0.85m,
        decimal bh = 0.10m,
        decimal tncn = 0.05m) : IPayRegulationReadRepository
    {
        public Task<PayRegulationSnapshot?> FindByCodeAsync(
            string code,
            CancellationToken cancellationToken = default)
        {
            var value = code switch
            {
                PayRegulationCodes.ProbationTimeWageFactor => probation,
                PayRegulationCodes.BhEmployeeRate => bh,
                PayRegulationCodes.TncnTempRate => tncn,
                PayRegulationCodes.StandardWorkDaysDefault => 22m,
                _ => 0m
            };
            return Task.FromResult<PayRegulationSnapshot?>(new PayRegulationSnapshot(code, code, value));
        }
    }

    private sealed class FakeEmployeeRepo(bool probation) : IEmployeeReadRepository
    {
        public Task<IReadOnlyList<EmployeeSnapshot>> ListAsync(CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<EmployeeSnapshot>>([]);

        public Task<EmployeeSnapshot?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            EmployeeContractSnapshot? contract = probation
                ? new EmployeeContractSnapshot("PROBATION", new DateOnly(2027, 1, 1), null, true)
                : new EmployeeContractSnapshot("OFFICIAL", new DateOnly(2026, 1, 1), null, false);
            return Task.FromResult<EmployeeSnapshot?>(new EmployeeSnapshot(
                EmpId, "MNV-DEV", "Dev", null, null, null, null, null, null, null,
                contract, null, EmployeeStatus.Active));
        }

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

    private sealed class FakeTimRepo(
        TimesheetPeriodStatus status,
        decimal workDays,
        decimal unpaid,
        decimal paid,
        decimal ot15) : ITimesheetImportRepository
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
                        Guid.NewGuid(), EmpId, "MNV-DEV", workDays, ot15, 0, 0, 0, paid, unpaid, 0)
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
                    l.TimeWageFactor,
                    l.Ot15,
                    l.Ot20,
                    l.Ot30,
                    l.ContractAllowance,
                    l.MonthlyAllowance,
                    l.BhRate,
                    l.TncnRate,
                    l.BhAmount,
                    l.TncnAmount,
                    l.NetPay)).ToList()));
        }

        public Task MarkClosedAsync(
            string periodYm,
            string closedByIdpSubject,
            CancellationToken cancellationToken = default) =>
            Task.CompletedTask;
    }

    private sealed class FakeAllowance(decimal contract = 0, decimal monthly = 0) : IPayAllowanceRepository
    {
        public Task<IReadOnlyList<PayAllowanceCatalogSnapshot>> ListCatalogAsync(
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<PayAllowanceCatalogSnapshot>>([]);

        public Task<bool> IsActiveCodeAsync(string code, CancellationToken cancellationToken = default) =>
            Task.FromResult(true);

        public Task<decimal> SumContractAsync(Guid employeeId, CancellationToken cancellationToken = default) =>
            Task.FromResult(contract);

        public Task<decimal> SumMonthlyAsync(
            string periodYm,
            Guid employeeId,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(monthly);

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
            CancellationToken cancellationToken = default) =>
            Task.CompletedTask;
    }

    private sealed class FakeSalary(decimal amount = 0) : IPayContractSalaryRepository
    {
        public Task<decimal> GetAmountAsync(Guid employeeId, CancellationToken cancellationToken = default) =>
            Task.FromResult(amount);
    }
}
