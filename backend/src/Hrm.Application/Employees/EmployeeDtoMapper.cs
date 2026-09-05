using Hrm.Application.Employees.Dtos;
using Hrm.Domain.Employees.Repositories;

namespace Hrm.Application.Employees;

internal static class EmployeeDtoMapper
{
    public static EmployeeDto Map(
        EmployeeSnapshot snapshot,
        SeniorityDto? seniority) =>
        new(
            snapshot.Id,
            snapshot.EmployeeCode,
            snapshot.FullName,
            snapshot.Cccd,
            snapshot.EmailCty,
            snapshot.TaxId,
            snapshot.OrgUnitCode,
            snapshot.EducationLevelCode,
            snapshot.EducationLevelName,
            seniority,
            snapshot.Contract is null
                ? null
                : new EmployeeContractDto(
                    snapshot.Contract.ContractType,
                    snapshot.Contract.StartDate,
                    snapshot.Contract.EndDate,
                    snapshot.Contract.IsProbation),
            snapshot.LineManagerEmployeeId,
            snapshot.Status.ToString());
}
