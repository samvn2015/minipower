using Hrm.Domain.Employees;
using Hrm.Domain.Employees.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Hrm.Infrastructure.Persistence.Repositories;

internal static class EmployeeSnapshotQueries
{
    public static IQueryable<EmployeeSnapshot> SelectSnapshots(this IQueryable<Domain.Employees.Entities.Employee> query) =>
        query.Select(e => new EmployeeSnapshot(
            e.Id,
            e.EmployeeCode,
            e.FullName,
            e.Cccd,
            e.EmailCty,
            e.TaxId,
            e.OrgUnitCode,
            e.Contract == null
                ? null
                : new EmployeeContractSnapshot(
                    e.Contract.ContractType,
                    e.Contract.StartDate,
                    e.Contract.EndDate,
                    e.Contract.IsProbation),
            e.LineManagerEmployeeId,
            e.Status));
}
