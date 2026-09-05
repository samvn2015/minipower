namespace Hrm.Application.Employees.Dtos;

public sealed record EmployeeContractDto(
    string ContractType,
    DateOnly StartDate,
    DateOnly? EndDate,
    bool IsProbation);
