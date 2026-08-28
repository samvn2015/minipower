using Jarvis.Domain.Shared.Messaging;

namespace Hrm.Application.Employees.Commands;

public sealed record CreateEmployeeCommand(
    string? ActorIdpSubject,
    string EmployeeCode,
    string? FullName,
    string? Cccd,
    string? EmailCty,
    string? TaxId) : ICommand;

public sealed record EmployeeCreateResult(Guid Id, string EmployeeCode, string Status);
