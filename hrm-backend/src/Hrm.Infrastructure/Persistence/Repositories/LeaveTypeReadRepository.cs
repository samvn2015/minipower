using Hrm.Domain.Leave;
using Hrm.Domain.Leave.Repositories;
using Hrm.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Hrm.Infrastructure.Persistence.Repositories;

internal sealed class LeaveTypeReadRepository(AppDbContext db) : ILeaveTypeReadRepository
{
    public async Task<IReadOnlyList<LeaveTypeSnapshot>> ListActiveAsync(
        CancellationToken cancellationToken = default) =>
        await db.LeaveTypes.AsNoTracking()
            .Where(x => x.Status == LeaveTypeStatus.Active)
            .OrderBy(x => x.Code)
            .Select(x => new LeaveTypeSnapshot(
                x.Code, x.Name, x.DeductsAnnualBalance, x.RequiresCompanyTemplateFile, x.Status))
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

    public async Task<LeaveTypeSnapshot?> FindByCodeAsync(
        string code,
        CancellationToken cancellationToken = default) =>
        await db.LeaveTypes.AsNoTracking()
            .Where(x => x.Code == code)
            .Select(x => new LeaveTypeSnapshot(
                x.Code, x.Name, x.DeductsAnnualBalance, x.RequiresCompanyTemplateFile, x.Status))
            .FirstOrDefaultAsync(cancellationToken)
            .ConfigureAwait(false);
}
