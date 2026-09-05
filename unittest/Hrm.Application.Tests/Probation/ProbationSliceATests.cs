using Hrm.Application.Probation.Queries;
using Hrm.Domain.Employees;
using Hrm.Domain.Employees.Repositories;
using Hrm.Domain.Identity;
using Hrm.Domain.Identity.Repositories;
using Hrm.Domain.Probation;
using Jarvis.Domain.Shared.ExceptionHandling;

namespace Hrm.Application.Tests.Probation;

public sealed class ProbationContractFactsTests
{
    [Fact]
    public void ReadMilestones_UsesContractDates_Only()
    {
        var contract = new EmployeeContractSnapshot("PROBATION", new DateOnly(2026, 1, 1), new DateOnly(2026, 4, 1), true);
        var (start, end, complete) = ProbationContractFacts.ReadMilestones(contract);
        Assert.Equal(new DateOnly(2026, 1, 1), start);
        Assert.Equal(new DateOnly(2026, 4, 1), end);
        Assert.True(complete);
        Assert.Equal(new DateOnly(2026, 3, 17), ProbationContractFacts.ComputeT15Date(end!.Value));
        Assert.Equal(new DateOnly(2026, 3, 25), ProbationContractFacts.ComputeT7Date(end.Value));
    }

    [Fact]
    public void ReadMilestones_MissingEnd_DoesNotInvent()
    {
        var contract = new EmployeeContractSnapshot("PROBATION", new DateOnly(2026, 1, 1), null, true);
        var (_, end, complete) = ProbationContractFacts.ReadMilestones(contract);
        Assert.Null(end);
        Assert.False(complete);
    }

    [Fact]
    public void IsActiveProbation_Official_False()
    {
        var contract = new EmployeeContractSnapshot("OFFICIAL", new DateOnly(2026, 1, 1), null, false);
        Assert.False(ProbationContractFacts.IsActiveProbationContract(contract));
    }
}

public sealed class ListProbationCasesQueryHandlerTests
{
    [Fact]
    public async Task HandleAsync_Hr_ListsOnlyActiveProbation()
    {
        var handler = new ListProbationCasesQueryHandler(
            new FakeAccounts(["IAM-ROLE-HR"], "MNV-HR"),
            new FakeEmployees([
                Emp("MNV-TV", EmployeeStatus.Active, "PROBATION", new DateOnly(2026, 1, 1), new DateOnly(2026, 6, 1), true),
                Emp("MNV-OFF", EmployeeStatus.Active, "OFFICIAL", new DateOnly(2025, 1, 1), null, false),
                Emp("MNV-LEFT", EmployeeStatus.Inactive, "PROBATION", new DateOnly(2026, 1, 1), new DateOnly(2026, 6, 1), true),
            ]));

        var rows = await handler.HandleAsync(new ListProbationCasesQuery("local-dev"));
        Assert.Single(rows);
        Assert.Equal("MNV-TV", rows[0].EmployeeCode);
        Assert.Equal(new DateOnly(2026, 6, 1), rows[0].ProbationEndDate);
        Assert.Equal(new DateOnly(2026, 5, 17), rows[0].T15DueDate);
    }

    [Fact]
    public async Task HandleAsync_Nv_Forbidden()
    {
        var handler = new ListProbationCasesQueryHandler(
            new FakeAccounts(["IAM-ROLE-NV"], "MNV-DEV"),
            new FakeEmployees([]));

        await Assert.ThrowsAsync<ForbiddenException>(() =>
            handler.HandleAsync(new ListProbationCasesQuery("local-nv")));
    }

    private static EmployeeSnapshot Emp(
        string code,
        EmployeeStatus status,
        string type,
        DateOnly start,
        DateOnly? end,
        bool isProbation) =>
        new(
            Guid.NewGuid(),
            code,
            code,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            new EmployeeContractSnapshot(type, start, end, isProbation),
            null,
            status);

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
}
