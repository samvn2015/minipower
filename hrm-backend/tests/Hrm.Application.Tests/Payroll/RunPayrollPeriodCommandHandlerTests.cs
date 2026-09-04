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

    private static RunPayrollPeriodCommandHandler CreateHandler(
        FakePayRepo pay,
        FakeTimRepo tim,
        FakeEmployeeRepo emp,
        FakeRegRepo? reg = null,
        FakeAllowance? allowance = null,
        FakeSalary? salary = null,
        FakeCalendar? calendar = null,
        FakeAccountRepo? account = null) =>
        new(
            account ?? new FakeAccountRepo(["IAM-ROLE-HR"]),
            tim,
            pay,
            emp,
            reg ?? new FakeRegRepo(),
            calendar ?? new FakeCalendar(26m),
            allowance ?? new FakeAllowance(),
            salary ?? new FakeSalary());

    [Fact]
    public async Task HandleAsync_TimClosed_CreatesDraftWithNTinh()
    {
        var pay = new FakePayRepo();
        var handler = CreateHandler(
            pay,
            new FakeTimRepo(TimesheetPeriodStatus.Closed, workDays: 22, unpaid: 2, paid: 2, ot15: 1),
            new FakeEmployeeRepo(probation: false));

        var result = await handler.HandleAsync(new RunPayrollPeriodCommand("local-dev", "2027-07"));

        Assert.Equal("Draft", result.Status);
        Assert.Equal(1, result.LineCount);
        Assert.NotNull(pay.LastLines);
        Assert.Equal(20m, pay.LastLines![0].NTinh); // 22 − 2; không + phép hưởng 2
        Assert.Equal(1.00m, pay.LastLines[0].TimeWageFactor);
        Assert.Equal(1m, pay.LastLines[0].Ot15); // OT từ TIM (FR-004)
        Assert.Equal(0m, pay.LastLines[0].ContractAllowance);
        Assert.Equal(0m, pay.LastLines[0].MonthlyAllowance);
        Assert.Contains(result.Warnings, w => w.Contains("A-001", StringComparison.Ordinal));
        Assert.Contains(result.Warnings, w => w.Contains("PAY-FR-013", StringComparison.Ordinal));
    }

    [Fact]
    public async Task HandleAsync_NoPaidLeave_NoA001Warning()
    {
        var handler = CreateHandler(
            new FakePayRepo(),
            new FakeTimRepo(TimesheetPeriodStatus.Closed, 20, 0, 0, 0),
            new FakeEmployeeRepo(false));

        var result = await handler.HandleAsync(new RunPayrollPeriodCommand("local-dev", "2029-03"));
        Assert.Empty(result.Warnings);
    }

    [Fact]
    public async Task HandleAsync_SumsContractAndMonthlyAllowances()
    {
        var pay = new FakePayRepo();
        var handler = CreateHandler(
            pay,
            new FakeTimRepo(TimesheetPeriodStatus.Closed, 20, 0, 0, 0),
            new FakeEmployeeRepo(probation: false),
            allowance: new FakeAllowance(contract: 730_000m, monthly: 200_000m));

        await handler.HandleAsync(new RunPayrollPeriodCommand("local-dev", "2027-12"));

        Assert.Equal(730_000m, pay.LastLines![0].ContractAllowance);
        Assert.Equal(200_000m, pay.LastLines[0].MonthlyAllowance);
    }

    [Fact]
    public async Task HandleAsync_UsesCbMasterRatesNotLegacyFlat()
    {
        var pay = new FakePayRepo();
        var handler = CreateHandler(
            pay,
            new FakeTimRepo(TimesheetPeriodStatus.Closed, 26, 0, 0, 0),
            new FakeEmployeeRepo(probation: false),
            salary: new FakeSalary(10_000_000m));

        await handler.HandleAsync(new RunPayrollPeriodCommand("local-dev", "2028-01"));

        var line = pay.LastLines![0];
        var expected = PayrollStatutoryCalculator.Compute(
            10_000_000m, 1m, 26m, 26m, 0m, 0m,
            0.08m, 0.015m, 0.01m,
            11_000_000m, 0, 4_400_000m, 0m);
        Assert.Equal(expected.BhRate, line.BhRate);
        Assert.Equal(expected.BhAmount, line.BhAmount);
        Assert.Equal(expected.TncnAmount, line.TncnAmount);
        Assert.Equal(expected.NetPay, line.NetPay);
    }

    [Fact]
    public async Task HandleAsync_ProbationContract_AppliesMasterFactor()
    {
        var pay = new FakePayRepo();
        var handler = CreateHandler(
            pay,
            new FakeTimRepo(TimesheetPeriodStatus.Closed, 20, 0, 0, 0),
            new FakeEmployeeRepo(probation: true));

        await handler.HandleAsync(new RunPayrollPeriodCommand("local-dev", "2027-07"));

        Assert.Equal(0.85m, pay.LastLines![0].TimeWageFactor);
    }

    [Fact]
    public async Task HandleAsync_TimNotClosed_ThrowsBadRequest()
    {
        var handler = CreateHandler(
            new FakePayRepo(),
            new FakeTimRepo(TimesheetPeriodStatus.Draft, 20, 0, 0, 0),
            new FakeEmployeeRepo(false));

        await Assert.ThrowsAsync<BadRequestException>(() =>
            handler.HandleAsync(new RunPayrollPeriodCommand("local-dev", "2027-07")));
    }

    [Fact]
    public async Task HandleAsync_LmForbidden()
    {
        var handler = CreateHandler(
            new FakePayRepo(),
            new FakeTimRepo(TimesheetPeriodStatus.Closed, 20, 0, 0, 0),
            new FakeEmployeeRepo(false),
            account: new FakeAccountRepo(["IAM-ROLE-LM"]));

        await Assert.ThrowsAsync<ForbiddenException>(() =>
            handler.HandleAsync(new RunPayrollPeriodCommand("local-lm", "2027-07")));
    }

    [Fact]
    public async Task HandleAsync_ClosedPeriod_ThrowsConflict()
    {
        var pay = new FakePayRepo(closed: true);
        var handler = CreateHandler(
            pay,
            new FakeTimRepo(TimesheetPeriodStatus.Closed, 20, 0, 0, 0),
            new FakeEmployeeRepo(false));

        var ex = await Assert.ThrowsAsync<ConflictException>(() =>
            handler.HandleAsync(new RunPayrollPeriodCommand("local-dev", "2028-05")));
        Assert.Contains("PAY-FR-016", ex.SystemMessage ?? ex.Message, StringComparison.Ordinal);
        Assert.Equal(1, pay.RunCount);
    }

    [Fact]
    public async Task HandleAsync_DraftPeriod_Overwrites()
    {
        var pay = new FakePayRepo();
        var handler = CreateHandler(
            pay,
            new FakeTimRepo(TimesheetPeriodStatus.Closed, 20, 0, 0, 0),
            new FakeEmployeeRepo(false));

        await handler.HandleAsync(new RunPayrollPeriodCommand("local-dev", "2028-05"));
        await handler.HandleAsync(new RunPayrollPeriodCommand("local-dev", "2028-05"));
        Assert.Equal(2, pay.RunCount);
        Assert.Equal("Draft", pay.LastStatus);
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

    private sealed class FakeRegRepo(decimal probation = 0.85m) : IPayRegulationReadRepository
    {
        public Task<PayRegulationSnapshot?> FindByCodeAsync(
            string code,
            CancellationToken cancellationToken = default)
        {
            decimal? value = code switch
            {
                PayRegulationCodes.ProbationTimeWageFactor => probation,
                PayRegulationCodes.BhxhEmployeeRate => 0.08m,
                PayRegulationCodes.BhytEmployeeRate => 0.015m,
                PayRegulationCodes.BhtnEmployeeRate => 0.01m,
                PayRegulationCodes.TncnPersonalDeduction => 11_000_000m,
                PayRegulationCodes.TncnDependentUnit => 4_400_000m,
                PayRegulationCodes.StandardWorkDaysDefault => 26m,
                _ => null
            };
            return Task.FromResult(
                value is null ? null : new PayRegulationSnapshot(code, code, value.Value));
        }
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

    private sealed class FakePayRepo(bool closed = false) : IPayPeriodRepository
    {
        public IReadOnlyList<PayLineCreateModel>? LastLines { get; private set; }
        public int RunCount { get; private set; }
        public Guid? LastPeriodId { get; private set; }
        public string? LastStatus { get; private set; }

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
            CancellationToken cancellationToken = default)
        {
            RunCount++;
            LastLines = lines;
            if (closed)
                return Task.FromResult<PayPeriodSnapshot?>(null);

            var id = Guid.NewGuid();
            LastPeriodId = id;
            LastStatus = nameof(PayPeriodStatus.Draft);
            return Task.FromResult<PayPeriodSnapshot?>(new PayPeriodSnapshot(
                id,
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

        public Task<PayPayslipSnapshot?> FindPayslipByLineIdAsync(
            Guid lineId,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<PayPayslipSnapshot?>(null);

        public Task<IReadOnlyList<PayPayslipSnapshot>> ListClosedPayslipsByEmployeeCodeAsync(
            string employeeCode,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<PayPayslipSnapshot>>([]);
    }

    private sealed class FakeAllowance(
        decimal contract = 0,
        decimal monthly = 0,
        decimal meal = 0,
        decimal advance = 0) : IPayAllowanceRepository
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

        public Task<decimal> SumMealTaxExemptAsync(
            string periodYm,
            Guid employeeId,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(meal);

        public Task<decimal> SumAdvanceAsync(
            string periodYm,
            Guid employeeId,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(advance);

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

    private sealed class FakeSalary(decimal amount = 0, int dependents = 0) : IPayContractSalaryRepository
    {
        public Task<decimal> GetAmountAsync(Guid employeeId, CancellationToken cancellationToken = default) =>
            Task.FromResult(amount);

        public Task<PayContractSalarySnapshot?> FindAsync(
            Guid employeeId,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<PayContractSalarySnapshot?>(
                amount == 0 && dependents == 0
                    ? null
                    : new PayContractSalarySnapshot(employeeId, "MNV-DEV", amount, dependents));

        public Task UpsertAsync(
            Guid employeeId,
            string employeeCode,
            decimal amount,
            int dependentCount,
            CancellationToken cancellationToken = default) =>
            Task.CompletedTask;
    }
}
