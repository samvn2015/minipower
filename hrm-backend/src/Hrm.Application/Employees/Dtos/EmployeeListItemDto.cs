namespace Hrm.Application.Employees.Dtos;

public sealed record EmployeeListItemDto(
    Guid Id,
    string EmployeeCode,
    string? FullName,
    string? EmailCty,
    string Status);
