namespace Hrm.Domain.Leave.Repositories;

public sealed record LeaveBalanceSnapshot(
    Guid Id,
    Guid EmployeeId,
    int Year,
    decimal EntitledDays,
    decimal UsedDays,
    decimal RemainingDays);

public interface ILeaveBalanceRepository
{
    Task<LeaveBalanceSnapshot?> FindByEmployeeAndYearAsync(
        Guid employeeId,
        int year,
        CancellationToken cancellationToken = default);
}
