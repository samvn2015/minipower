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
using Hrm.Application.Payroll.Commands;
using Hrm.Application.Payroll.Dtos;
using Hrm.Application.Payroll.Queries;
using Hrm.Application.Timekeeping;
using Hrm.Application.Timekeeping.Commands;
using Hrm.Application.Timekeeping.Dtos;
using Hrm.Application.Timekeeping.Queries;
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
        builder.Services.AddScoped<IAsyncQueryHandler<ListPendingLeaveRequestsC1Query, IReadOnlyList<LeaveRequestPendingC1Dto>>, ListPendingLeaveRequestsC1QueryHandler>();
        builder.Services.AddScoped<IAsyncCommandHandler<CreateLeaveRequestCommand, LeaveRequestCreateResult>, CreateLeaveRequestCommandHandler>();
        builder.Services.AddScoped<IAsyncCommandHandler<ApproveLeaveRequestC1Command, LeaveRequestActionResult>, ApproveLeaveRequestC1CommandHandler>();
        builder.Services.AddScoped<IAsyncCommandHandler<RejectLeaveRequestC1Command, LeaveRequestActionResult>, RejectLeaveRequestC1CommandHandler>();
        builder.Services.AddScoped<IAsyncQueryHandler<ListPendingLeaveRequestsC2Query, IReadOnlyList<LeaveRequestPendingC1Dto>>, ListPendingLeaveRequestsC2QueryHandler>();
        builder.Services.AddScoped<IAsyncCommandHandler<ApproveLeaveRequestC2Command, LeaveRequestActionResult>, ApproveLeaveRequestC2CommandHandler>();
        builder.Services.AddScoped<IAsyncCommandHandler<RejectLeaveRequestC2Command, LeaveRequestActionResult>, RejectLeaveRequestC2CommandHandler>();
        builder.Services.AddScoped<IAsyncCommandHandler<CancelLeaveRequestCommand, LeaveRequestActionResult>, CancelLeaveRequestCommandHandler>();

        builder.Services.AddScoped<IAsyncQueryHandler<GetActiveTimesheetTemplateQuery, TimesheetTemplateDto?>, GetActiveTimesheetTemplateQueryHandler>();
        builder.Services.AddScoped<IAsyncQueryHandler<ListTimesheetTemplatesQuery, IReadOnlyList<TimesheetTemplateDto>>, ListTimesheetTemplatesQueryHandler>();
        builder.Services.AddScoped<IAsyncCommandHandler<CreateTimesheetTemplateCommand, TimesheetTemplateCreateResult>, CreateTimesheetTemplateCommandHandler>();
        builder.Services.AddScoped<IAsyncCommandHandler<PublishTimesheetTemplateCommand, TimesheetTemplatePublishResult>, PublishTimesheetTemplateCommandHandler>();
        builder.Services.AddScoped<IAsyncCommandHandler<PreviewTimesheetImportCommand, TimesheetImportBatchDto>, PreviewTimesheetImportCommandHandler>();
        builder.Services.AddScoped<IAsyncCommandHandler<CommitTimesheetImportCommand, TimesheetCommitResult>, CommitTimesheetImportCommandHandler>();
        builder.Services.AddScoped<IAsyncCommandHandler<CloseTimesheetPeriodCommand, TimesheetCloseResult>, CloseTimesheetPeriodCommandHandler>();
        builder.Services.AddScoped<IAsyncCommandHandler<UnlockTimesheetPeriodCommand, TimesheetUnlockResult>, UnlockTimesheetPeriodCommandHandler>();
        builder.Services.AddScoped<IAsyncQueryHandler<GetTimesheetImportBatchQuery, TimesheetImportBatchDto>, GetTimesheetImportBatchQueryHandler>();
        builder.Services.AddScoped<IAsyncQueryHandler<ListTimesheetPeriodsQuery, IReadOnlyList<TimesheetPeriodDto>>, ListTimesheetPeriodsQueryHandler>();
        builder.Services.AddScoped<IAsyncQueryHandler<GetTimesheetPeriodQuery, TimesheetPeriodDto>, GetTimesheetPeriodQueryHandler>();

        builder.Services.AddScoped<IAsyncCommandHandler<RunPayrollPeriodCommand, PayRunResult>, RunPayrollPeriodCommandHandler>();
        builder.Services.AddScoped<IAsyncCommandHandler<ClosePayrollPeriodCommand, PayRunResult>, ClosePayrollPeriodCommandHandler>();
        builder.Services.AddScoped<IAsyncCommandHandler<UpsertPayWorkdayCalendarCommand, PayWorkdayCalendarResult>, UpsertPayWorkdayCalendarCommandHandler>();
        builder.Services.AddScoped<IAsyncQueryHandler<GetPayrollPeriodQuery, PayPeriodDto>, GetPayrollPeriodQueryHandler>();
        builder.Services.AddScoped<IAsyncQueryHandler<ListPayrollPeriodsQuery, IReadOnlyList<PayPeriodDto>>, ListPayrollPeriodsQueryHandler>();

        return builder;
    }
}
