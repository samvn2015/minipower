using Hrm.Domain.Employees;
using Hrm.Domain.Employees.Repositories;

namespace Hrm.Application.Tests.Employees;

internal static class EmpTestSnapshots
{
    public static EmployeeSnapshot DevEmployee(
        Guid id,
        string code = "MNV-DEV",
        string? fullName = "Dev IAM",
        string? email = "dev@company.local",
        string? org = "ORG-HQ",
        string? educationCode = null,
        string? educationName = null,
        DateOnly? seniorityStart = null,
        EmployeeContractSnapshot? contract = null,
        Guid? lineManagerId = null,
        EmployeeStatus status = EmployeeStatus.Active) =>
        new(
            id,
            code,
            fullName,
            null,
            email,
            null,
            org,
            educationCode,
            educationName,
            seniorityStart,
            contract,
            lineManagerId,
            status);
}

internal sealed class FakeEducationLevelRepo : IEducationLevelReadRepository
{
    private readonly HashSet<string> _active;

    public FakeEducationLevelRepo(params string[] activeCodes) =>
        _active = activeCodes.Length == 0
            ? new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "EDU-THPT", "EDU-DH" }
            : activeCodes.ToHashSet(StringComparer.OrdinalIgnoreCase);

    public Task<bool> IsActiveAsync(string code, CancellationToken cancellationToken = default) =>
        Task.FromResult(_active.Contains(code));

    public Task<IReadOnlyList<EducationLevelSnapshot>> ListActiveAsync(
        CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyList<EducationLevelSnapshot>>(
            _active.Select(c => new EducationLevelSnapshot(c, c)).ToArray());
}

internal sealed class FakeSeniorityRuleRepo : ISeniorityRuleReadRepository
{
    public Task<SeniorityRuleSnapshot?> GetActiveAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult<SeniorityRuleSnapshot?>(
            new SeniorityRuleSnapshot("SR-DEFAULT", SeniorityBasisType.ContractStartDate));
}

internal sealed class FakeAuditLogRepo : IEmpAuditLogRepository
{
    public List<EmpAuditLogEntry> Entries { get; } = [];

    public Task AppendAsync(EmpAuditLogEntry entry, CancellationToken cancellationToken = default)
    {
        Entries.Add(entry);
        return Task.CompletedTask;
    }

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
