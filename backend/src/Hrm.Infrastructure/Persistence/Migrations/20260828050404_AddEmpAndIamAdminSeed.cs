using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hrm.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddEmpAndIamAdminSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "emp_employee",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeCode = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    FullName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Cccd = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    EmailCty = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    TaxId = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    LineManagerEmployeeId = table.Column<Guid>(type: "uuid", nullable: true),
                    Status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_emp_employee", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "emp_employee",
                columns: new[] { "Id", "Cccd", "EmailCty", "EmployeeCode", "FullName", "LineManagerEmployeeId", "Status", "TaxId" },
                values: new object[] { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), null, "dev@company.local", "MNV-DEV", "Dev IAM", null, "Active", null });

            migrationBuilder.InsertData(
                table: "iam_identity_account",
                columns: new[] { "Id", "DisplayName", "EmailCty", "EmployeeCode", "IdpSubject", "Status" },
                values: new object[] { new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"), "IT Dev", "it@company.local", null, "it-dev", "Active" });

            migrationBuilder.InsertData(
                table: "iam_account_role",
                columns: new[] { "AccountId", "RoleCode" },
                values: new object[] { new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"), "IAM-ROLE-IT" });

            migrationBuilder.CreateIndex(
                name: "IX_emp_employee_Cccd",
                table: "emp_employee",
                column: "Cccd",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_emp_employee_EmailCty",
                table: "emp_employee",
                column: "EmailCty",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_emp_employee_EmployeeCode",
                table: "emp_employee",
                column: "EmployeeCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_emp_employee_TaxId",
                table: "emp_employee",
                column: "TaxId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "emp_employee");

            migrationBuilder.DeleteData(
                table: "iam_account_role",
                keyColumns: new[] { "AccountId", "RoleCode" },
                keyValues: new object[] { new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"), "IAM-ROLE-IT" });

            migrationBuilder.DeleteData(
                table: "iam_identity_account",
                keyColumn: "Id",
                keyValue: new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"));
        }
    }
}
