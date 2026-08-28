using Jarvis.Domain.Shared.Messaging;

namespace Hrm.Application.Identity.Admin.Queries;

public sealed record ListIdentityAccountsQuery(string? ActorIdpSubject) : IQuery;
