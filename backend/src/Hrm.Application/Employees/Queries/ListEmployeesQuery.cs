using Jarvis.Domain.Shared.Messaging;

namespace Hrm.Application.Employees.Queries;

/// <summary>S1 — <c>Page</c>/<c>Size</c> tuỳ chọn; null = mặc định có trần (PageRequest).</summary>
public sealed record ListEmployeesQuery(string? ActorIdpSubject, int? Page = null, int? Size = null) : IQuery;
