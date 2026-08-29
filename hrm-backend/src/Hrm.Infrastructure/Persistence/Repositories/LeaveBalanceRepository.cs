using Hrm.Domain.Leave.Repositories;
using Hrm.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Hrm.Infrastructure.Persistence.Repositories;

internal sealed class LeaveBalanceRepository(AppDbContext db) : ILeaveBalanceRepository
{
    public async Task<LeaveBalanceSnapshot?> FindByEmployeeAndYearAsync(
        Guid employeeId,
        int year,
        CancellationToken cancellationToken = default) =>
        await db.LeaveBalances.AsNoTracking()
            .Where(x => x.EmployeeId == employeeId && x.Year == year)
            .Select(x => new LeaveBalanceSnapshot(
                x.Id,
                x.EmployeeId,
                x.Year,
                x.EntitledDays,
                x.UsedDays,
                x.EntitledDays - x.UsedDays))
            .FirstOrDefaultAsync(cancellationToken)
            .ConfigureAwait(false);
}
