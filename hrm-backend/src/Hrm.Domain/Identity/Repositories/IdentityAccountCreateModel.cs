namespace Hrm.Domain.Identity.Repositories;

public sealed record IdentityAccountCreateModel(
    string IdpSubject,
    string? DisplayName,
    string? EmailCty,
    string EmployeeCode,
    IReadOnlyList<string> InitialRoleCodes);
