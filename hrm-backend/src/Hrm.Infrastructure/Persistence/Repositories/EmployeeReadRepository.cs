using Hrm.Domain.Employees;
using Hrm.Domain.Employees.Entities;
using Hrm.Domain.Employees.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Hrm.Infrastructure.Persistence.Repositories;

internal sealed class EmployeeReadRepository(AppDbContext db) : IEmployeeReadRepository
{
    public async Task<IReadOnlyList<EmployeeSnapshot>> ListAsync(
        CancellationToken cancellationToken = default)
    {
        return await db.Employees
            .AsNoTracking()
            .OrderBy(e => e.EmployeeCode)
            .SelectSnapshots()
            .ToListAsync(cancellationToken);
    }

    public async Task<EmployeeSnapshot?> FindByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await db.Employees
            .AsNoTracking()
            .Where(e => e.Id == id)
            .SelectSnapshots()
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
            .SelectSnapshots()
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<EmployeeSnapshot?> FindByEmailCtyAsync(
        string emailCty,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(emailCty))
            return null;

        var normalized = emailCty.Trim().ToLowerInvariant();
        return await db.Employees
            .AsNoTracking()
            .Where(e => e.EmailCty != null && e.EmailCty.ToLower() == normalized)
            .SelectSnapshots()
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<EmployeeUniqueField?> FindDuplicateAsync(
        string employeeCode,
        string? cccd,
        string? emailCty,
        string? taxId,
        Guid? excludeEmployeeId = null,
        CancellationToken cancellationToken = default)
    {
        if (!string.IsNullOrWhiteSpace(employeeCode)
            && await ExistsAsync(e => e.EmployeeCode == employeeCode.Trim(), excludeEmployeeId, cancellationToken))
        {
            return EmployeeUniqueField.EmployeeCode;
        }

        if (!string.IsNullOrWhiteSpace(cccd)
            && await ExistsAsync(e => e.Cccd == cccd.Trim(), excludeEmployeeId, cancellationToken))
        {
            return EmployeeUniqueField.Cccd;
        }

        if (!string.IsNullOrWhiteSpace(emailCty)
            && await ExistsAsync(e => e.EmailCty == emailCty.Trim(), excludeEmployeeId, cancellationToken))
        {
            return EmployeeUniqueField.EmailCty;
        }

        if (!string.IsNullOrWhiteSpace(taxId)
            && await ExistsAsync(e => e.TaxId == taxId.Trim(), excludeEmployeeId, cancellationToken))
        {
            return EmployeeUniqueField.TaxId;
        }

        return null;
    }

    private async Task<bool> ExistsAsync(
        System.Linq.Expressions.Expression<Func<Employee, bool>> predicate,
        Guid? excludeEmployeeId,
        CancellationToken cancellationToken)
    {
        var query = db.Employees.AsNoTracking().Where(predicate);
        if (excludeEmployeeId.HasValue)
            query = query.Where(e => e.Id != excludeEmployeeId.Value);

        return await query.AnyAsync(cancellationToken);
    }
}
