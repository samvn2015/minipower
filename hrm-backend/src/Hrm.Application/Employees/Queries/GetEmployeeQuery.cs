using Jarvis.Domain.Shared.Messaging;

namespace Hrm.Application.Employees.Queries;

public sealed record GetEmployeeQuery(Guid EmployeeId, string? ActorIdpSubject) : IQuery;
