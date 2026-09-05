using Hrm.Application.Employees.Dtos;
using Jarvis.Domain.Shared.Messaging;

namespace Hrm.Application.Employees.Commands;

/// <summary>EMP-SCR-005 — HR gửi đề xuất đổi LM.</summary>
public sealed record SubmitLineManagerChangeCommand(
    string? ActorIdpSubject,
    Guid EmployeeId,
    Guid ProposedLineManagerEmployeeId) : ICommand;

public sealed record LineManagerChangeResult(Guid RequestId, string Status);
