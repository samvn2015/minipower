using Hrm.Domain.Employees;
using Hrm.Domain.Employees.Entities;
using Hrm.Domain.Employees.Repositories;
using Hrm.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Hrm.Infrastructure.Persistence.Repositories;

internal sealed class EmployeeWriteRepository(AppDbContext db, IAppUnitOfWork unitOfWork)
    : IEmployeeWriteRepository
{
    public async Task<Guid> CreateAsync(
        EmployeeCreateModel model,
        CancellationToken cancellationToken = default)
    {
        var entity = new Employee
        {
            Id = Guid.NewGuid(),
            EmployeeCode = model.EmployeeCode.Trim(),
            FullName = model.FullName?.Trim(),
            Cccd = model.Cccd?.Trim(),
            EmailCty = model.EmailCty?.Trim(),
            TaxId = model.TaxId?.Trim(),
            Status = EmployeeStatus.Active
        };

        db.Employees.Add(entity);
        await unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return entity.Id;
    }

    public async Task<bool> UpdateAsync(
        Guid id,
        EmployeePatch patch,
        CancellationToken cancellationToken = default)
    {
        var employee = await db.Employees.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
        if (employee is null)
            return false;

        if (patch.FullName is not null)
            employee.FullName = patch.FullName.Trim();
        if (patch.EmailCty is not null)
            employee.EmailCty = patch.EmailCty.Trim();
        if (patch.Cccd is not null)
            employee.Cccd = patch.Cccd.Trim();
        if (patch.TaxId is not null)
            employee.TaxId = patch.TaxId.Trim();

        await unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return true;
    }
}
