using Jarvis.Domain.Shared.Messaging;

namespace Hrm.Application.Identity.Admin.Commands;

public sealed record AssignAccountRoleCommand(
    Guid AccountId,
    string RoleCode,
    string? ActorIdpSubject) : ICommand;

public sealed record RemoveAccountRoleCommand(
    Guid AccountId,
    string RoleCode,
    string? ActorIdpSubject) : ICommand;

public sealed record DisableIdentityAccountCommand(
    Guid AccountId,
    string? ActorIdpSubject) : ICommand;

public sealed record IdentityAccountAdminResult(
    Guid AccountId,
    string Status,
    IReadOnlyList<string> Roles);
