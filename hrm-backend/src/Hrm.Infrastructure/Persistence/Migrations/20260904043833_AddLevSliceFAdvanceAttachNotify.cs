using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hrm.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddLevSliceFAdvanceAttachNotify : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "RequiresCompanyTemplateFile",
                table: "lev_leave_type",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "AttachmentFileName",
                table: "lev_leave_request",
                type: "character varying(512)",
                maxLength: 512,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "AttachmentMatchesCompanyTemplate",
                table: "lev_leave_request",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "lev_notification_outbox",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    LeaveRequestId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    EventType = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Channel = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    Message = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_lev_notification_outbox", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "lev_leave_type",
                keyColumn: "Code",
                keyValue: "LEV-ANNUAL",
                column: "RequiresCompanyTemplateFile",
                value: false);

            migrationBuilder.UpdateData(
                table: "lev_leave_type",
                keyColumn: "Code",
                keyValue: "LEV-BEREAVEMENT",
                column: "RequiresCompanyTemplateFile",
                value: false);

            migrationBuilder.UpdateData(
                table: "lev_leave_type",
                keyColumn: "Code",
                keyValue: "LEV-MARRIAGE",
                column: "RequiresCompanyTemplateFile",
                value: false);

            migrationBuilder.UpdateData(
                table: "lev_leave_type",
                keyColumn: "Code",
                keyValue: "LEV-MATERNITY",
                column: "RequiresCompanyTemplateFile",
                value: false);

            migrationBuilder.UpdateData(
                table: "lev_leave_type",
                keyColumn: "Code",
                keyValue: "LEV-SICK",
                column: "RequiresCompanyTemplateFile",
                value: true);

            migrationBuilder.UpdateData(
                table: "lev_leave_type",
                keyColumn: "Code",
                keyValue: "LEV-UNPAID",
                column: "RequiresCompanyTemplateFile",
                value: false);

            migrationBuilder.CreateIndex(
                name: "IX_lev_notification_outbox_EmployeeId",
                table: "lev_notification_outbox",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_lev_notification_outbox_LeaveRequestId",
                table: "lev_notification_outbox",
                column: "LeaveRequestId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "lev_notification_outbox");

            migrationBuilder.DropColumn(
                name: "RequiresCompanyTemplateFile",
                table: "lev_leave_type");

            migrationBuilder.DropColumn(
                name: "AttachmentFileName",
                table: "lev_leave_request");

            migrationBuilder.DropColumn(
                name: "AttachmentMatchesCompanyTemplate",
                table: "lev_leave_request");
        }
    }
}
