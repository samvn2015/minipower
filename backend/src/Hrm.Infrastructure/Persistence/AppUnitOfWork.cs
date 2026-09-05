using Hrm.Domain.Repositories;
using Jarvis.Domain.DataStorages;
using Jarvis.EntityFramework.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Hrm.Infrastructure.Persistence;

public sealed class AppUnitOfWork(
    IServiceProvider services,
    IDbContextFactory<AppDbContext> factory,
    ITenantIdResolverFactory tenantIdResolverFactory,
    ICurrentTenantAccessor currentTenantAccessor)
    : BaseUnitOfWork<AppDbContext>(services, factory, tenantIdResolverFactory, currentTenantAccessor),
        IAppUnitOfWork;
