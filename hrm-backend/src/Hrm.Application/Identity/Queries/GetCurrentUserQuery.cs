using Jarvis.Domain.Shared.Messaging;

namespace Hrm.Application.Identity.Queries;

/// <summary>
/// IAM-AC-001 — map claim đã extract từ Host (không đọc HttpContext trong Application).
/// </summary>
public sealed record GetCurrentUserQuery(
    string? Subject,
    string? Name,
    IReadOnlyList<string> RoleClaims) : IQuery;
