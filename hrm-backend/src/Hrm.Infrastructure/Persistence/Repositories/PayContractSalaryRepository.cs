using Hrm.Domain.Payroll.Entities;
using Hrm.Domain.Payroll.Repositories;
using Hrm.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Hrm.Infrastructure.Persistence.Repositories;

internal sealed class PayContractSalaryRepository(AppDbContext db) : IPayContractSalaryRepository
{
    public async Task<decimal> GetAmountAsync(Guid employeeId, CancellationToken cancellationToken = default)
    {
        return await db.PayContractSalaries.AsNoTracking()
            .Where(x => x.EmployeeId == employeeId)
            .Select(x => x.Amount)
            .FirstOrDefaultAsync(cancellationToken)
            .ConfigureAwait(false);
    }
}
