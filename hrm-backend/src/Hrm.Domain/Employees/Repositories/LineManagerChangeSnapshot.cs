using Hrm.Domain.Employees;

namespace Hrm.Domain.Employees.Repositories;

public sealed record LineManagerChangeSnapshot(
    Guid Id,
    Guid EmployeeId,
    string EmployeeCode,
    string? EmployeeFullName,
    Guid ProposedLineManagerEmployeeId,
    string ProposedLineManagerCode,
    string? ProposedLineManagerName,
    LineManagerChangeStatus Status,
    string RequestedByIdpSubject,
    DateTime RequestedAtUtc,
    string? ReviewedByIdpSubject,
    DateTime? ReviewedAtUtc,
    string? ReviewNote);
