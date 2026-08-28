namespace Hrm.Domain.Identity.Repositories;

public sealed record IdentityAccountSnapshot(
    Guid AccountId,
    string IdpSubject,
    string? DisplayName,
    string? EmailCty,
    string? EmployeeCode,
    IdentityAccountStatus Status,
    IReadOnlyList<string> RoleCodes);
