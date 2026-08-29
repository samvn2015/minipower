using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Hrm.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddLevSliceB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "lev_leave_balance",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    EntitledDays = table.Column<decimal>(type: "numeric(5,1)", precision: 5, scale: 1, nullable: false),
                    UsedDays = table.Column<decimal>(type: "numeric(5,1)", precision: 5, scale: 1, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_lev_leave_balance", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "lev_leave_type",
                columns: table => new
                {
                    Code = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    Name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    DeductsAnnualBalance = table.Column<bool>(type: "boolean", nullable: false),
                    Status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_lev_leave_type", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "lev_leave_request",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    LeaveTypeCode = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    FromDate = table.Column<DateOnly>(type: "date", nullable: false),
                    ToDate = table.Column<DateOnly>(type: "date", nullable: false),
                    DayPart = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    TotalDays = table.Column<decimal>(type: "numeric(5,1)", precision: 5, scale: 1, nullable: false),
                    Reason = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    HandoverEmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    IsEmergency = table.Column<bool>(type: "boolean", nullable: false),
                    SubmittedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_lev_leave_request", x => x.Id);
                    table.ForeignKey(
                        name: "FK_lev_leave_request_lev_leave_type_LeaveTypeCode",
                        column: x => x.LeaveTypeCode,
                        principalTable: "lev_leave_type",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "emp_employee",
                columns: new[] { "Id", "Cccd", "EducationLevelCode", "EmailCty", "EmployeeCode", "FullName", "LineManagerEmployeeId", "OrgUnitCode", "SeniorityStartDate", "Status", "TaxId" },
                values: new object[] { new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"), null, null, "handover@company.local", "MNV-HO", "Handover NV", null, "ORG-HQ", null, "Active", null });

            migrationBuilder.InsertData(
                table: "lev_leave_balance",
                columns: new[] { "Id", "EmployeeId", "EntitledDays", "UsedDays", "Year" },
                values: new object[] { new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"), new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), 12m, 0m, 2026 });

            migrationBuilder.InsertData(
                table: "lev_leave_type",
                columns: new[] { "Code", "DeductsAnnualBalance", "Name", "Status" },
                values: new object[,]
                {
                    { "LEV-ANNUAL", true, "Phép năm", "Active" },
                    { "LEV-BEREAVEMENT", false, "Phép tang chế", "Active" },
                    { "LEV-MARRIAGE", false, "Phép kết hôn", "Active" },
                    { "LEV-MATERNITY", false, "Nghỉ chế độ Nam/Nữ", "Active" },
                    { "LEV-SICK", false, "Phép ốm/BHXH", "Active" },
                    { "LEV-UNPAID", false, "Phép không hưởng lương", "Active" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_lev_leave_balance_EmployeeId_Year",
                table: "lev_leave_balance",
                columns: new[] { "EmployeeId", "Year" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_lev_leave_request_EmployeeId",
                table: "lev_leave_request",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_lev_leave_request_LeaveTypeCode",
                table: "lev_leave_request",
                column: "LeaveTypeCode");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "lev_leave_balance");

            migrationBuilder.DropTable(
                name: "lev_leave_request");

            migrationBuilder.DropTable(
                name: "lev_leave_type");

            migrationBuilder.DeleteData(
                table: "emp_employee",
                keyColumn: "Id",
                keyValue: new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"));
        }
    }
}
