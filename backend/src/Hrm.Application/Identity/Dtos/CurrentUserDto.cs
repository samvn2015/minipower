namespace Hrm.Application.Identity.Dtos;

/// <summary>
/// Response <c>GET /v1/iam/me</c> (DOC-12). Roles từ IAM DB (ADR-002).
/// </summary>
public sealed record CurrentUserDto(
    string? Sub,
    string? Name,
    IReadOnlyList<string> Roles,
    string? Note);
