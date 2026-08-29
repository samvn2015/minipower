using Hrm.Application.Employees;
using Hrm.Application.Employees.Commands;
using Hrm.Application.Employees.Dtos;
using Hrm.Application.Employees.Queries;
using Hrm.Application.Identity;
using Hrm.Application.Identity.Admin.Commands;
using Hrm.Application.Identity.Admin.Dtos;
using Hrm.Application.Identity.Admin.Queries;
using Hrm.Application.Identity.Dtos;
using Hrm.Application.Identity.Queries;
using Hrm.Application.Leave.Commands;
using Hrm.Application.Leave.Dtos;
using Hrm.Application.Leave.Queries;
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

        builder.Services.AddScoped<IdentityAccountProvisioner>();
        builder.Services.AddScoped<IAsyncQueryHandler<GetCurrentUserQuery, CurrentUserDto>, GetCurrentUserQueryHandler>();
        builder.Services.AddScoped<IAsyncQueryHandler<ListIdentityAccountsQuery, IReadOnlyList<IdentityAccountDto>>, ListIdentityAccountsQueryHandler>();
        builder.Services.AddScoped<IAsyncQueryHandler<GetIdentityAccountQuery, IdentityAccountDto>, GetIdentityAccountQueryHandler>();
        builder.Services.AddScoped<EmployeeDtoFactory>();
        builder.Services.AddScoped<IAsyncQueryHandler<GetEmployeeQuery, EmployeeDto>, GetEmployeeQueryHandler>();
        builder.Services.AddScoped<IAsyncQueryHandler<GetMyEmployeeQuery, EmployeeDto>, GetMyEmployeeQueryHandler>();
        builder.Services.AddScoped<IAsyncQueryHandler<ListEmployeesQuery, IReadOnlyList<EmployeeListItemDto>>, ListEmployeesQueryHandler>();
        builder.Services.AddScoped<IAsyncQueryHandler<ListEducationLevelsQuery, IReadOnlyList<EducationLevelDto>>, ListEducationLevelsQueryHandler>();
        builder.Services.AddScoped<IAsyncQueryHandler<ListEmployeeAuditLogsQuery, IReadOnlyList<EmpAuditLogDto>>, ListEmployeeAuditLogsQueryHandler>();

        builder.Services.AddScoped<IAsyncCommandHandler<AssignAccountRoleCommand, IdentityAccountAdminResult>, AssignAccountRoleCommandHandler>();
        builder.Services.AddScoped<IAsyncCommandHandler<RemoveAccountRoleCommand, IdentityAccountAdminResult>, RemoveAccountRoleCommandHandler>();
        builder.Services.AddScoped<IAsyncCommandHandler<DisableIdentityAccountCommand, IdentityAccountAdminResult>, DisableIdentityAccountCommandHandler>();
        builder.Services.AddScoped<IAsyncCommandHandler<CreateEmployeeCommand, EmployeeCreateResult>, CreateEmployeeCommandHandler>();
        builder.Services.AddScoped<IAsyncCommandHandler<UpdateEmployeeCommand, EmployeeUpdateResult>, UpdateEmployeeCommandHandler>();
        builder.Services.AddScoped<IAsyncCommandHandler<SubmitLineManagerChangeCommand, LineManagerChangeResult>, SubmitLineManagerChangeCommandHandler>();
        builder.Services.AddScoped<IAsyncCommandHandler<ApproveLineManagerChangeCommand, LineManagerChangeResult>, ApproveLineManagerChangeCommandHandler>();
        builder.Services.AddScoped<IAsyncCommandHandler<RejectLineManagerChangeCommand, LineManagerChangeResult>, RejectLineManagerChangeCommandHandler>();
        builder.Services.AddScoped<IAsyncQueryHandler<ListPendingLineManagerChangesQuery, IReadOnlyList<LineManagerChangeDto>>, ListPendingLineManagerChangesQueryHandler>();

        builder.Services.AddScoped<IAsyncQueryHandler<ListLeaveTypesQuery, IReadOnlyList<LeaveTypeDto>>, ListLeaveTypesQueryHandler>();
        builder.Services.AddScoped<IAsyncQueryHandler<GetMyLeaveBalanceQuery, LeaveBalanceDto>, GetMyLeaveBalanceQueryHandler>();
        builder.Services.AddScoped<IAsyncQueryHandler<ListMyLeaveRequestsQuery, IReadOnlyList<LeaveRequestDto>>, ListMyLeaveRequestsQueryHandler>();
        builder.Services.AddScoped<IAsyncCommandHandler<CreateLeaveRequestCommand, LeaveRequestCreateResult>, CreateLeaveRequestCommandHandler>();

        return builder;
    }
}
