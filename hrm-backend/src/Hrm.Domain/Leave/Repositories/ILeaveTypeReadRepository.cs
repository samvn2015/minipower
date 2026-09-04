namespace Hrm.Domain.Leave.Repositories;

public sealed record LeaveTypeSnapshot(
    string Code,
    string Name,
    bool DeductsAnnualBalance,
    bool RequiresCompanyTemplateFile,
    LeaveTypeStatus Status);

public interface ILeaveTypeReadRepository
{
    Task<IReadOnlyList<LeaveTypeSnapshot>> ListActiveAsync(CancellationToken cancellationToken = default);

    Task<LeaveTypeSnapshot?> FindByCodeAsync(string code, CancellationToken cancellationToken = default);
}
