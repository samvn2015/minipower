namespace Hrm.Domain.Employees.Repositories;

public sealed record EmployeeContractSnapshot(
    string ContractType,
    DateOnly StartDate,
    DateOnly? EndDate,
    bool IsProbation);
