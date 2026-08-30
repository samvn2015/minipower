using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Hrm.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddEmpEducationSeniorityAudit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EducationLevelCode",
                table: "emp_employee",
                type: "character varying(32)",
                maxLength: 32,
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "SeniorityStartDate",
                table: "emp_employee",
                type: "date",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "emp_audit_log",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Action = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: true),
                    RelatedId = table.Column<Guid>(type: "uuid", nullable: true),
                    ActorIdpSubject = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    OccurredAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Detail = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_emp_audit_log", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "emp_education_level",
                columns: table => new
                {
                    Code = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    Name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_emp_education_level", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "emp_seniority_rule",
                columns: table => new
                {
                    Code = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    BasisType = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    Status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_emp_seniority_rule", x => x.Code);
                });

            migrationBuilder.InsertData(
                table: "emp_education_level",
                columns: new[] { "Code", "Name", "Status" },
                values: new object[,]
                {
                    { "EDU-CD", "Cao đẳng", "Active" },
                    { "EDU-DH", "Đại học", "Active" },
                    { "EDU-INACTIVE", "Ngừng hiệu lực (test)", "Inactive" },
                    { "EDU-THPT", "Trung học phổ thông", "Active" }
                });

            migrationBuilder.UpdateData(
                table: "emp_employee",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                columns: new[] { "EducationLevelCode", "SeniorityStartDate" },
                values: new object[] { null, null });

            migrationBuilder.InsertData(
                table: "emp_seniority_rule",
                columns: new[] { "Code", "BasisType", "Status" },
                values: new object[] { "SR-DEFAULT", "ContractStartDate", "Active" });

            migrationBuilder.CreateIndex(
                name: "IX_emp_employee_EducationLevelCode",
                table: "emp_employee",
                column: "EducationLevelCode");

            migrationBuilder.CreateIndex(
                name: "IX_emp_audit_log_EmployeeId",
                table: "emp_audit_log",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_emp_audit_log_OccurredAtUtc",
                table: "emp_audit_log",
                column: "OccurredAtUtc");

            migrationBuilder.AddForeignKey(
                name: "FK_emp_employee_emp_education_level_EducationLevelCode",
                table: "emp_employee",
                column: "EducationLevelCode",
                principalTable: "emp_education_level",
                principalColumn: "Code",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_emp_employee_emp_education_level_EducationLevelCode",
                table: "emp_employee");

            migrationBuilder.DropTable(
                name: "emp_audit_log");

            migrationBuilder.DropTable(
                name: "emp_education_level");

            migrationBuilder.DropTable(
                name: "emp_seniority_rule");

            migrationBuilder.DropIndex(
                name: "IX_emp_employee_EducationLevelCode",
                table: "emp_employee");

            migrationBuilder.DropColumn(
                name: "EducationLevelCode",
                table: "emp_employee");

            migrationBuilder.DropColumn(
                name: "SeniorityStartDate",
                table: "emp_employee");
        }
    }
}
