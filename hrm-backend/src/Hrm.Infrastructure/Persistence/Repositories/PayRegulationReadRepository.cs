using Hrm.Domain.Payroll.Entities;
using Hrm.Domain.Payroll.Repositories;
using Hrm.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Hrm.Infrastructure.Persistence.Repositories;

internal sealed class PayRegulationReadRepository(AppDbContext db) : IPayRegulationReadRepository
{
    public async Task<PayRegulationSnapshot?> FindByCodeAsync(
        string code,
        CancellationToken cancellationToken = default)
    {
        return await db.PayRegulations.AsNoTracking()
            .Where(x => x.Code == code)
            .Select(x => new PayRegulationSnapshot(x.Code, x.Name, x.DecimalValue))
            .FirstOrDefaultAsync(cancellationToken)
            .ConfigureAwait(false);
    }
}
