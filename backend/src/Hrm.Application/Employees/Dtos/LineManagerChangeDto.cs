using Hrm.Domain.Employees.Repositories;

namespace Hrm.Application.Employees.Dtos;

public sealed record LineManagerChangeDto(
    Guid Id,
    Guid EmployeeId,
    string EmployeeCode,
    string? EmployeeFullName,
    Guid ProposedLineManagerEmployeeId,
    string ProposedLineManagerCode,
    string? ProposedLineManagerName,
    string Status,
    string RequestedByIdpSubject,
    DateTime RequestedAtUtc,
    string? ReviewedByIdpSubject,
    DateTime? ReviewedAtUtc,
    string? ReviewNote)
{
    internal static LineManagerChangeDto FromSnapshot(LineManagerChangeSnapshot snapshot) =>
        new(
            snapshot.Id,
            snapshot.EmployeeId,
            snapshot.EmployeeCode,
            snapshot.EmployeeFullName,
            snapshot.ProposedLineManagerEmployeeId,
            snapshot.ProposedLineManagerCode,
            snapshot.ProposedLineManagerName,
            snapshot.Status.ToString(),
            snapshot.RequestedByIdpSubject,
            snapshot.RequestedAtUtc,
            snapshot.ReviewedByIdpSubject,
            snapshot.ReviewedAtUtc,
            snapshot.ReviewNote);
}
