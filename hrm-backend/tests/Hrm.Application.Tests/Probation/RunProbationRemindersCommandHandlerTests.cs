using Hrm.Application.Probation.Commands;
using Hrm.Domain.Employees;
using Hrm.Domain.Employees.Repositories;
using Hrm.Domain.Identity;
using Hrm.Domain.Identity.Repositories;
using Hrm.Domain.Probation;
using Hrm.Domain.Probation.Repositories;

namespace Hrm.Application.Tests.Probation;

public sealed class RunProbationRemindersCommandHandlerTests
{
    [Fact]
    public async Task HandleAsync_AsOfT15_CreatesAlert_NotCrm()
    {
        var empId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
        var repo = new FakeReminderRepo();
        var handler = new RunProbationRemindersCommandHandler(
            new FakeAccounts(["IAM-ROLE-HR"], "MNV-HR"),
            new FakeEmployees([
                new EmployeeSnapshot(
                    empId, "MNV-TV", "TV", null, "tv@company.local", null, null, null, null, null,
                    new EmployeeContractSnapshot("PROBATION", new DateOnly(2026, 1, 1), new DateOnly(2026, 6, 30), true),
                    null,
                    EmployeeStatus.Active)
            ]),
            repo);

        // KT 2026-06-30 → T-15 = 2026-06-15
        var result = await handler.HandleAsync(
            new RunProbationRemindersCommand("local-dev", new DateOnly(2026, 6, 15)));

        Assert.Equal(1, result.T15Created);
        Assert.Equal(0, result.T7Created);
        Assert.Single(repo.Items);
        Assert.Equal(ProbationReminderKind.T15, repo.Items[0].Kind);
        Assert.Equal(RunProbationRemindersCommandHandler.ChannelInAppAndEmail, repo.Items[0].Channel);
        Assert.DoesNotContain("crm", repo.Items[0].Channel, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("sales", repo.Items[0].EmailTo, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task HandleAsync_AsOfT7_NoLm_AssignsHrPool()
    {
        var empId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");
        var repo = new FakeReminderRepo();
        var handler = new RunProbationRemindersCommandHandler(
            new FakeAccounts(["IAM-ROLE-HR"], "MNV-HR"),
            new FakeEmployees([
                new EmployeeSnapshot(
                    empId, "MNV-TV2", "TV2", null, "tv2@company.local", null, null, null, null, null,
                    new EmployeeContractSnapshot("PROBATION", new DateOnly(2026, 1, 1), new DateOnly(2026, 6, 30), true),
                    null,
                    EmployeeStatus.Active)
            ]),
            repo);

        // T-7 = 2026-06-23
        var result = await handler.HandleAsync(
            new RunProbationRemindersCommand("local-dev", new DateOnly(2026, 6, 23)));

        Assert.Equal(1, result.T7Created);
        Assert.Null(repo.Items[0].AssigneeEmployeeId);
        Assert.Contains("HR", repo.Items[0].InAppMessage);
    }

    [Fact]
    public async Task HandleAsync_MissingEndDate_SkippedNoInvent()
    {
        var empId = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc");
        var repo = new FakeReminderRepo();
        var handler = new RunProbationRemindersCommandHandler(
            new FakeAccounts(["IAM-ROLE-HR"], "MNV-HR"),
            new FakeEmployees([
                new EmployeeSnapshot(
                    empId, "MNV-BAD", "BAD", null, null, null, null, null, null, null,
                    new EmployeeContractSnapshot("PROBATION", new DateOnly(2026, 1, 1), null, true),
                    null,
                    EmployeeStatus.Active)
            ]),
            repo);

        var result = await handler.HandleAsync(
            new RunProbationRemindersCommand("local-dev", new DateOnly(2026, 6, 15)));

        Assert.Equal(0, result.T15Created);
        Assert.Equal(1, result.SkippedIncompleteMilestone);
        Assert.Empty(repo.Items);
    }

    private sealed class FakeAccounts(string[] roles, string? employeeCode) : IIdentityAccountReadRepository
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

    private sealed class FakeEmployees(IReadOnlyList<EmployeeSnapshot> items) : IEmployeeReadRepository
    {
        public Task<IReadOnlyList<EmployeeSnapshot>> ListAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult(items);

        public Task<EmployeeSnapshot?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
            Task.FromResult(items.FirstOrDefault(e => e.Id == id));

        public Task<EmployeeSnapshot?> FindByEmployeeCodeAsync(
            string employeeCode,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(items.FirstOrDefault(e =>
                string.Equals(e.EmployeeCode, employeeCode, StringComparison.OrdinalIgnoreCase)));

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

    private sealed class FakeReminderRepo : IProbationReminderRepository
    {
        public List<ProbationReminderCreateModel> Items { get; } = [];

        public Task<bool> ExistsAsync(
            Guid employeeId,
            ProbationReminderKind kind,
            DateOnly probationEndDate,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(Items.Any(x =>
                x.EmployeeId == employeeId && x.Kind == kind && x.ProbationEndDate == probationEndDate));

        public Task AddManyAsync(
            IReadOnlyList<ProbationReminderCreateModel> rows,
            CancellationToken cancellationToken = default)
        {
            Items.AddRange(rows);
            return Task.CompletedTask;
        }

        public Task<IReadOnlyList<ProbationReminderSnapshot>> ListAsync(
            ProbationReminderKind? kind = null,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<ProbationReminderSnapshot>>([]);
    }
}
