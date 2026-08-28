using Jarvis.Domain.Shared.Messaging;

namespace Hrm.Application.Employees.Commands;

public sealed record UpdateEmployeeCommand(
    Guid EmployeeId,
    string? ActorIdpSubject,
    string? FullName,
    string? EmailCty,
    string? Cccd) : ICommand;

public sealed record EmployeeUpdateResult(Guid EmployeeId, string Status);
