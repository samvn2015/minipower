using Hrm.Domain.Employees;
using Hrm.Domain.Employees.Entities;
using Hrm.Domain.Employees.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Hrm.Infrastructure.Persistence.Repositories;

internal sealed class EmployeeWriteRepository(AppDbContext db)
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
            OrgUnitCode = model.OrgUnitCode?.Trim(),
            Status = EmployeeStatus.Active
        };

        db.Employees.Add(entity);

        if (model.Contract is not null)
        {
            db.EmployeeContracts.Add(MapContract(entity.Id, model.Contract));
        }

        await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return entity.Id;
    }

    public async Task<bool> UpdateAsync(
        Guid id,
        EmployeePatch patch,
        CancellationToken cancellationToken = default)
    {
        var employee = await db.Employees
            .Include(e => e.Contract)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
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
        if (patch.OrgUnitCode is not null)
            employee.OrgUnitCode = patch.OrgUnitCode.Trim();

        if (patch.Contract is not null)
        {
            if (employee.Contract is null)
            {
                db.EmployeeContracts.Add(MapContract(id, patch.Contract));
            }
            else
            {
                ApplyContract(employee.Contract, patch.Contract);
            }
        }

        await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return true;
    }

    public async Task SetLineManagerAsync(
        Guid employeeId,
        Guid lineManagerEmployeeId,
        CancellationToken cancellationToken = default)
    {
        var employee = await db.Employees.FirstOrDefaultAsync(e => e.Id == employeeId, cancellationToken);
        if (employee is null)
            throw new InvalidOperationException($"Employee {employeeId} không tồn tại.");

        employee.LineManagerEmployeeId = lineManagerEmployeeId;
        await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    private static EmployeeContract MapContract(Guid employeeId, EmployeeContractUpsert contract) =>
        new()
        {
            Id = Guid.NewGuid(),
            EmployeeId = employeeId,
            ContractType = contract.ContractType.Trim(),
            StartDate = contract.StartDate,
            EndDate = contract.EndDate,
            IsProbation = contract.IsProbation
        };

    private static void ApplyContract(EmployeeContract entity, EmployeeContractUpsert contract)
    {
        entity.ContractType = contract.ContractType.Trim();
        entity.StartDate = contract.StartDate;
        entity.EndDate = contract.EndDate;
        entity.IsProbation = contract.IsProbation;
    }
}
