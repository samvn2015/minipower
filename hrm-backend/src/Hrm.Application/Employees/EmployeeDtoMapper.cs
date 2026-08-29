using Hrm.Domain.Employees.Repositories;

namespace Hrm.Application.Employees;

internal static class EmployeeDtoMapper
{
    public static Dtos.EmployeeDto Map(EmployeeSnapshot snapshot) =>
        new(
            snapshot.Id,
            snapshot.EmployeeCode,
            snapshot.FullName,
            snapshot.Cccd,
            snapshot.EmailCty,
            snapshot.TaxId,
            snapshot.OrgUnitCode,
            snapshot.Contract is null
                ? null
                : new Dtos.EmployeeContractDto(
                    snapshot.Contract.ContractType,
                    snapshot.Contract.StartDate,
                    snapshot.Contract.EndDate,
                    snapshot.Contract.IsProbation),
            snapshot.LineManagerEmployeeId,
            snapshot.Status.ToString());
}
