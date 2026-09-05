using Jarvis.Domain.Shared.Messaging;

namespace Hrm.Application.Employees.Queries;

public sealed record ListEmployeesQuery(string? ActorIdpSubject) : IQuery;
