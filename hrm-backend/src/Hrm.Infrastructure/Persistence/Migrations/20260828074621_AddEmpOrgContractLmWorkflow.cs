using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Hrm.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddEmpOrgContractLmWorkflow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OrgUnitCode",
                table: "emp_employee",
                type: "character varying(32)",
                maxLength: 32,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "emp_contract",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    ContractType = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: true),
                    IsProbation = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_emp_contract", x => x.Id);
                    table.ForeignKey(
                        name: "FK_emp_contract_emp_employee_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "emp_employee",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "emp_line_manager_change",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProposedLineManagerEmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    RequestedByIdpSubject = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    RequestedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ReviewedByIdpSubject = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ReviewedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ReviewNote = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_emp_line_manager_change", x => x.Id);
                    table.ForeignKey(
                        name: "FK_emp_line_manager_change_emp_employee_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "emp_employee",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "emp_org_unit",
                columns: table => new
                {
                    Code = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_emp_org_unit", x => x.Code);
                });

            migrationBuilder.UpdateData(
                table: "emp_employee",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                column: "OrgUnitCode",
                value: "ORG-HQ");

            migrationBuilder.InsertData(
                table: "emp_org_unit",
                columns: new[] { "Code", "Name", "Status" },
                values: new object[,]
                {
                    { "ORG-HQ", "Trụ sở HN", "Active" },
                    { "ORG-INACTIVE", "Đơn vị ngừng", "Inactive" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_emp_employee_OrgUnitCode",
                table: "emp_employee",
                column: "OrgUnitCode");

            migrationBuilder.CreateIndex(
                name: "IX_emp_contract_EmployeeId",
                table: "emp_contract",
                column: "EmployeeId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_emp_line_manager_change_EmployeeId_Status",
                table: "emp_line_manager_change",
                columns: new[] { "EmployeeId", "Status" });

            migrationBuilder.AddForeignKey(
                name: "FK_emp_employee_emp_org_unit_OrgUnitCode",
                table: "emp_employee",
                column: "OrgUnitCode",
                principalTable: "emp_org_unit",
                principalColumn: "Code",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_emp_employee_emp_org_unit_OrgUnitCode",
                table: "emp_employee");

            migrationBuilder.DropTable(
                name: "emp_contract");

            migrationBuilder.DropTable(
                name: "emp_line_manager_change");

            migrationBuilder.DropTable(
                name: "emp_org_unit");

            migrationBuilder.DropIndex(
                name: "IX_emp_employee_OrgUnitCode",
                table: "emp_employee");

            migrationBuilder.DropColumn(
                name: "OrgUnitCode",
                table: "emp_employee");
        }
    }
}
