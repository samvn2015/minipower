using Hrm.Domain.Employees.Repositories;
using Jarvis.Domain.Shared.Messaging;

namespace Hrm.Application.Employees.Commands;

public sealed record CreateEmployeeCommand(
    string? ActorIdpSubject,
    string EmployeeCode,
    string? FullName,
    string? Cccd,
    string? EmailCty,
    string? TaxId,
    string? OrgUnitCode,
    EmployeeContractUpsert? Contract) : ICommand;

public sealed record EmployeeCreateResult(
    Guid Id,
    string EmployeeCode,
    string Status,
    IReadOnlyList<string> Warnings);
