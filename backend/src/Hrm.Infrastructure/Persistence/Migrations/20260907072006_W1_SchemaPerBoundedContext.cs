using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hrm.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class W1_SchemaPerBoundedContext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "shared");

            migrationBuilder.EnsureSchema(
                name: "emp");

            migrationBuilder.EnsureSchema(
                name: "iam");

            migrationBuilder.EnsureSchema(
                name: "lev");

            migrationBuilder.EnsureSchema(
                name: "lif");

            migrationBuilder.EnsureSchema(
                name: "pay");

            migrationBuilder.EnsureSchema(
                name: "prb");

            migrationBuilder.EnsureSchema(
                name: "tim");

            migrationBuilder.RenameTable(
                name: "tim_timesheet_line",
                newName: "tim_timesheet_line",
                newSchema: "tim");

            migrationBuilder.RenameTable(
                name: "tim_template_version",
                newName: "tim_template_version",
                newSchema: "tim");

            migrationBuilder.RenameTable(
                name: "tim_template_column",
                newName: "tim_template_column",
                newSchema: "tim");

            migrationBuilder.RenameTable(
                name: "tim_period",
                newName: "tim_period",
                newSchema: "tim");

            migrationBuilder.RenameTable(
                name: "tim_import_row",
                newName: "tim_import_row",
                newSchema: "tim");

            migrationBuilder.RenameTable(
                name: "tim_import_batch",
                newName: "tim_import_batch",
                newSchema: "tim");

            migrationBuilder.RenameTable(
                name: "prb_reminder",
                newName: "prb_reminder",
                newSchema: "prb");

            migrationBuilder.RenameTable(
                name: "prb_outcome",
                newName: "prb_outcome",
                newSchema: "prb");

            migrationBuilder.RenameTable(
                name: "prb_extend_duration",
                newName: "prb_extend_duration",
                newSchema: "prb");

            migrationBuilder.RenameTable(
                name: "prb_evaluation",
                newName: "prb_evaluation",
                newSchema: "prb");

            migrationBuilder.RenameTable(
                name: "prb_criterion",
                newName: "prb_criterion",
                newSchema: "prb");

            migrationBuilder.RenameTable(
                name: "pay_workday_calendar",
                newName: "pay_workday_calendar",
                newSchema: "pay");

            migrationBuilder.RenameTable(
                name: "pay_regulation",
                newName: "pay_regulation",
                newSchema: "pay");

            migrationBuilder.RenameTable(
                name: "pay_period",
                newName: "pay_period",
                newSchema: "pay");

            migrationBuilder.RenameTable(
                name: "pay_monthly_allowance",
                newName: "pay_monthly_allowance",
                newSchema: "pay");

            migrationBuilder.RenameTable(
                name: "pay_line",
                newName: "pay_line",
                newSchema: "pay");

            migrationBuilder.RenameTable(
                name: "pay_export_outbox",
                newName: "pay_export_outbox",
                newSchema: "pay");

            migrationBuilder.RenameTable(
                name: "pay_contract_salary",
                newName: "pay_contract_salary",
                newSchema: "pay");

            migrationBuilder.RenameTable(
                name: "pay_contract_allowance",
                newName: "pay_contract_allowance",
                newSchema: "pay");

            migrationBuilder.RenameTable(
                name: "pay_allowance_catalog",
                newName: "pay_allowance_catalog",
                newSchema: "pay");

            migrationBuilder.RenameTable(
                name: "lif_onboarding_case",
                newName: "lif_onboarding_case",
                newSchema: "lif");

            migrationBuilder.RenameTable(
                name: "lif_on_checklist_tick",
                newName: "lif_on_checklist_tick",
                newSchema: "lif");

            migrationBuilder.RenameTable(
                name: "lif_on_checklist_item",
                newName: "lif_on_checklist_item",
                newSchema: "lif");

            migrationBuilder.RenameTable(
                name: "lif_offboarding_case",
                newName: "lif_offboarding_case",
                newSchema: "lif");

            migrationBuilder.RenameTable(
                name: "lif_off_checklist_tick",
                newName: "lif_off_checklist_tick",
                newSchema: "lif");

            migrationBuilder.RenameTable(
                name: "lif_off_checklist_item",
                newName: "lif_off_checklist_item",
                newSchema: "lif");

            migrationBuilder.RenameTable(
                name: "lif_access_lock_outbox",
                newName: "lif_access_lock_outbox",
                newSchema: "lif");

            migrationBuilder.RenameTable(
                name: "lev_notification_outbox",
                newName: "lev_notification_outbox",
                newSchema: "lev");

            migrationBuilder.RenameTable(
                name: "lev_leave_type",
                newName: "lev_leave_type",
                newSchema: "lev");

            migrationBuilder.RenameTable(
                name: "lev_leave_request",
                newName: "lev_leave_request",
                newSchema: "lev");

            migrationBuilder.RenameTable(
                name: "lev_leave_balance",
                newName: "lev_leave_balance",
                newSchema: "lev");

            migrationBuilder.RenameTable(
                name: "iam_role",
                newName: "iam_role",
                newSchema: "iam");

            migrationBuilder.RenameTable(
                name: "iam_identity_account",
                newName: "iam_identity_account",
                newSchema: "iam");

            migrationBuilder.RenameTable(
                name: "iam_account_role",
                newName: "iam_account_role",
                newSchema: "iam");

            migrationBuilder.RenameTable(
                name: "emp_seniority_rule",
                newName: "emp_seniority_rule",
                newSchema: "emp");

            migrationBuilder.RenameTable(
                name: "emp_org_unit",
                newName: "emp_org_unit",
                newSchema: "emp");

            migrationBuilder.RenameTable(
                name: "emp_line_manager_change",
                newName: "emp_line_manager_change",
                newSchema: "emp");

            migrationBuilder.RenameTable(
                name: "emp_employee",
                newName: "emp_employee",
                newSchema: "emp");

            migrationBuilder.RenameTable(
                name: "emp_education_level",
                newName: "emp_education_level",
                newSchema: "emp");

            migrationBuilder.RenameTable(
                name: "emp_contract",
                newName: "emp_contract",
                newSchema: "emp");

            migrationBuilder.RenameTable(
                name: "emp_audit_log",
                newName: "emp_audit_log",
                newSchema: "shared");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "tim_timesheet_line",
                schema: "tim",
                newName: "tim_timesheet_line");

            migrationBuilder.RenameTable(
                name: "tim_template_version",
                schema: "tim",
                newName: "tim_template_version");

            migrationBuilder.RenameTable(
                name: "tim_template_column",
                schema: "tim",
                newName: "tim_template_column");

            migrationBuilder.RenameTable(
                name: "tim_period",
                schema: "tim",
                newName: "tim_period");

            migrationBuilder.RenameTable(
                name: "tim_import_row",
                schema: "tim",
                newName: "tim_import_row");

            migrationBuilder.RenameTable(
                name: "tim_import_batch",
                schema: "tim",
                newName: "tim_import_batch");

            migrationBuilder.RenameTable(
                name: "prb_reminder",
                schema: "prb",
                newName: "prb_reminder");

            migrationBuilder.RenameTable(
                name: "prb_outcome",
                schema: "prb",
                newName: "prb_outcome");

            migrationBuilder.RenameTable(
                name: "prb_extend_duration",
                schema: "prb",
                newName: "prb_extend_duration");

            migrationBuilder.RenameTable(
                name: "prb_evaluation",
                schema: "prb",
                newName: "prb_evaluation");

            migrationBuilder.RenameTable(
                name: "prb_criterion",
                schema: "prb",
                newName: "prb_criterion");

            migrationBuilder.RenameTable(
                name: "pay_workday_calendar",
                schema: "pay",
                newName: "pay_workday_calendar");

            migrationBuilder.RenameTable(
                name: "pay_regulation",
                schema: "pay",
                newName: "pay_regulation");

            migrationBuilder.RenameTable(
                name: "pay_period",
                schema: "pay",
                newName: "pay_period");

            migrationBuilder.RenameTable(
                name: "pay_monthly_allowance",
                schema: "pay",
                newName: "pay_monthly_allowance");

            migrationBuilder.RenameTable(
                name: "pay_line",
                schema: "pay",
                newName: "pay_line");

            migrationBuilder.RenameTable(
                name: "pay_export_outbox",
                schema: "pay",
                newName: "pay_export_outbox");

            migrationBuilder.RenameTable(
                name: "pay_contract_salary",
                schema: "pay",
                newName: "pay_contract_salary");

            migrationBuilder.RenameTable(
                name: "pay_contract_allowance",
                schema: "pay",
                newName: "pay_contract_allowance");

            migrationBuilder.RenameTable(
                name: "pay_allowance_catalog",
                schema: "pay",
                newName: "pay_allowance_catalog");

            migrationBuilder.RenameTable(
                name: "lif_onboarding_case",
                schema: "lif",
                newName: "lif_onboarding_case");

            migrationBuilder.RenameTable(
                name: "lif_on_checklist_tick",
                schema: "lif",
                newName: "lif_on_checklist_tick");

            migrationBuilder.RenameTable(
                name: "lif_on_checklist_item",
                schema: "lif",
                newName: "lif_on_checklist_item");

            migrationBuilder.RenameTable(
                name: "lif_offboarding_case",
                schema: "lif",
                newName: "lif_offboarding_case");

            migrationBuilder.RenameTable(
                name: "lif_off_checklist_tick",
                schema: "lif",
                newName: "lif_off_checklist_tick");

            migrationBuilder.RenameTable(
                name: "lif_off_checklist_item",
                schema: "lif",
                newName: "lif_off_checklist_item");

            migrationBuilder.RenameTable(
                name: "lif_access_lock_outbox",
                schema: "lif",
                newName: "lif_access_lock_outbox");

            migrationBuilder.RenameTable(
                name: "lev_notification_outbox",
                schema: "lev",
                newName: "lev_notification_outbox");

            migrationBuilder.RenameTable(
                name: "lev_leave_type",
                schema: "lev",
                newName: "lev_leave_type");

            migrationBuilder.RenameTable(
                name: "lev_leave_request",
                schema: "lev",
                newName: "lev_leave_request");

            migrationBuilder.RenameTable(
                name: "lev_leave_balance",
                schema: "lev",
                newName: "lev_leave_balance");

            migrationBuilder.RenameTable(
                name: "iam_role",
                schema: "iam",
                newName: "iam_role");

            migrationBuilder.RenameTable(
                name: "iam_identity_account",
                schema: "iam",
                newName: "iam_identity_account");

            migrationBuilder.RenameTable(
                name: "iam_account_role",
                schema: "iam",
                newName: "iam_account_role");

            migrationBuilder.RenameTable(
                name: "emp_seniority_rule",
                schema: "emp",
                newName: "emp_seniority_rule");

            migrationBuilder.RenameTable(
                name: "emp_org_unit",
                schema: "emp",
                newName: "emp_org_unit");

            migrationBuilder.RenameTable(
                name: "emp_line_manager_change",
                schema: "emp",
                newName: "emp_line_manager_change");

            migrationBuilder.RenameTable(
                name: "emp_employee",
                schema: "emp",
                newName: "emp_employee");

            migrationBuilder.RenameTable(
                name: "emp_education_level",
                schema: "emp",
                newName: "emp_education_level");

            migrationBuilder.RenameTable(
                name: "emp_contract",
                schema: "emp",
                newName: "emp_contract");

            migrationBuilder.RenameTable(
                name: "emp_audit_log",
                schema: "shared",
                newName: "emp_audit_log");
        }
    }
}
