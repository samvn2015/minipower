using Hrm.Domain.Employees.Repositories;
using Hrm.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Hrm.Infrastructure.Persistence.Repositories;

internal sealed class EmployeeWriteRepository(AppDbContext db, IAppUnitOfWork unitOfWork)
    : IEmployeeWriteRepository
{
    public async Task<bool> UpdateAsync(
        Guid id,
        EmployeePatch patch,
        CancellationToken cancellationToken = default)
    {
        var employee = await db.Employees.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
        if (employee is null)
            return false;

        if (patch.FullName is not null)
            employee.FullName = patch.FullName;
        if (patch.EmailCty is not null)
            employee.EmailCty = patch.EmailCty;
        if (patch.Cccd is not null)
            employee.Cccd = patch.Cccd;

        await unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return true;
    }
}
