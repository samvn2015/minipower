using Jarvis.Domain.Shared.Messaging;

namespace Hrm.Application.Identity.Admin.Queries;

public sealed record GetIdentityAccountQuery(Guid AccountId, string? ActorIdpSubject) : IQuery;
