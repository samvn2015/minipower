using Hrm.Domain.Employees.Repositories;
using Jarvis.Domain.Shared.Messaging;

namespace Hrm.Application.Employees.Commands;

public sealed record UpdateEmployeeCommand(
    Guid EmployeeId,
    string? ActorIdpSubject,
    string? FullName,
    string? EmailCty,
    string? Cccd,
    string? TaxId,
    string? OrgUnitCode,
    EmployeeContractUpsert? Contract) : ICommand;

public sealed record EmployeeUpdateResult(Guid EmployeeId, string Status);
