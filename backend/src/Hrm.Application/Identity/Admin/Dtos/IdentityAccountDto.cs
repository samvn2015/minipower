namespace Hrm.Application.Identity.Admin.Dtos;

public sealed record IdentityAccountDto(
    Guid Id,
    string IdpSubject,
    string? DisplayName,
    string? EmailCty,
    string? EmployeeCode,
    string Status,
    IReadOnlyList<string> Roles);
