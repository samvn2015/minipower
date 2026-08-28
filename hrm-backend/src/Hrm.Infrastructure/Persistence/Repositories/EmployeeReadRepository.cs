using Hrm.Domain.Employees.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Hrm.Infrastructure.Persistence.Repositories;

internal sealed class EmployeeReadRepository(AppDbContext db) : IEmployeeReadRepository
{
    public async Task<EmployeeSnapshot?> FindByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await db.Employees
            .AsNoTracking()
            .Where(e => e.Id == id)
            .Select(e => new EmployeeSnapshot(
                e.Id,
                e.EmployeeCode,
                e.FullName,
                e.Cccd,
                e.EmailCty,
                e.TaxId,
                e.LineManagerEmployeeId,
                e.Status))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<EmployeeSnapshot?> FindByEmployeeCodeAsync(
        string employeeCode,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(employeeCode))
            return null;

        return await db.Employees
            .AsNoTracking()
            .Where(e => e.EmployeeCode == employeeCode)
            .Select(e => new EmployeeSnapshot(
                e.Id,
                e.EmployeeCode,
                e.FullName,
                e.Cccd,
                e.EmailCty,
                e.TaxId,
                e.LineManagerEmployeeId,
                e.Status))
            .FirstOrDefaultAsync(cancellationToken);
    }
}
