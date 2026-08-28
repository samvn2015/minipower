using Hrm.Application.Employees.Commands;
using Hrm.Application.Employees.Dtos;
using Hrm.Application.Employees.Queries;
using Hrm.Application.Identity.Admin.Commands;
using Hrm.Application.Identity.Admin.Dtos;
using Hrm.Application.Identity.Admin.Queries;
using Hrm.Application.Identity.Dtos;
using Hrm.Application.Identity.Queries;
using Jarvis.Application;
using Jarvis.Application.Contracts.Commands;
using Jarvis.Application.Contracts.Queries;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Hrm.Application.DependencyInjection;

public static class ApplicationLayerExtension
{
    public static IHostApplicationBuilder AddApplicationLayer(this IHostApplicationBuilder builder)
    {
        builder.AddCoreApplication();

        builder.Services.AddScoped<IAsyncQueryHandler<GetCurrentUserQuery, CurrentUserDto>, GetCurrentUserQueryHandler>();
        builder.Services.AddScoped<IAsyncQueryHandler<ListIdentityAccountsQuery, IReadOnlyList<IdentityAccountDto>>, ListIdentityAccountsQueryHandler>();
        builder.Services.AddScoped<IAsyncQueryHandler<GetIdentityAccountQuery, IdentityAccountDto>, GetIdentityAccountQueryHandler>();
        builder.Services.AddScoped<IAsyncQueryHandler<GetEmployeeQuery, EmployeeDto>, GetEmployeeQueryHandler>();

        builder.Services.AddScoped<IAsyncCommandHandler<AssignAccountRoleCommand, IdentityAccountAdminResult>, AssignAccountRoleCommandHandler>();
        builder.Services.AddScoped<IAsyncCommandHandler<RemoveAccountRoleCommand, IdentityAccountAdminResult>, RemoveAccountRoleCommandHandler>();
        builder.Services.AddScoped<IAsyncCommandHandler<DisableIdentityAccountCommand, IdentityAccountAdminResult>, DisableIdentityAccountCommandHandler>();
        builder.Services.AddScoped<IAsyncCommandHandler<UpdateEmployeeCommand, EmployeeUpdateResult>, UpdateEmployeeCommandHandler>();

        return builder;
    }
}
