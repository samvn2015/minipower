using Hrm.Application.Employees.Dtos;
using Jarvis.Domain.Shared.Messaging;

namespace Hrm.Application.Employees.Queries;

/// <summary>EMP-SCR-003 — hồ sơ của tôi (self-service).</summary>
public sealed record GetMyEmployeeQuery(string? ActorIdpSubject) : IQuery;
