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

    public async Task<PayContractSalarySnapshot?> FindAsync(
        Guid employeeId,
        CancellationToken cancellationToken = default)
    {
        return await db.PayContractSalaries.AsNoTracking()
            .Where(x => x.EmployeeId == employeeId)
            .Select(x => new PayContractSalarySnapshot(x.EmployeeId, x.EmployeeCode, x.Amount, x.DependentCount))
            .FirstOrDefaultAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task UpsertAsync(
        Guid employeeId,
        string employeeCode,
        decimal amount,
        int dependentCount,
        CancellationToken cancellationToken = default)
    {
        var row = await db.PayContractSalaries
            .FirstOrDefaultAsync(x => x.EmployeeId == employeeId, cancellationToken)
            .ConfigureAwait(false);
        if (row is null)
        {
            db.PayContractSalaries.Add(new PayContractSalary
            {
                Id = Guid.NewGuid(),
                EmployeeId = employeeId,
                EmployeeCode = employeeCode,
                Amount = amount,
                DependentCount = dependentCount
            });
        }
        else
        {
            row.EmployeeCode = employeeCode;
            row.Amount = amount;
            row.DependentCount = dependentCount;
        }

        await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}
