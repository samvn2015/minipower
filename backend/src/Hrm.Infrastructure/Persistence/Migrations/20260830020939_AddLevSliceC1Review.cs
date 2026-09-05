using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Hrm.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddLevSliceC1Review : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "C1ReviewNote",
                table: "lev_leave_request",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "C1ReviewedAtUtc",
                table: "lev_leave_request",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "C1ReviewedByIdpSubject",
                table: "lev_leave_request",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "emp_employee",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                column: "LineManagerEmployeeId",
                value: new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"));

            migrationBuilder.InsertData(
                table: "iam_identity_account",
                columns: new[] { "Id", "DisplayName", "EmailCty", "EmployeeCode", "IdpSubject", "Status" },
                values: new object[] { new Guid("11111111-1111-1111-1111-111111111111"), "Handover NV (LM)", "handover@company.local", "MNV-HO", "local-lm", "Active" });

            migrationBuilder.InsertData(
                table: "iam_account_role",
                columns: new[] { "AccountId", "RoleCode" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), "IAM-ROLE-LM" },
                    { new Guid("11111111-1111-1111-1111-111111111111"), "IAM-ROLE-NV" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "iam_account_role",
                keyColumns: new[] { "AccountId", "RoleCode" },
                keyValues: new object[] { new Guid("11111111-1111-1111-1111-111111111111"), "IAM-ROLE-LM" });

            migrationBuilder.DeleteData(
                table: "iam_account_role",
                keyColumns: new[] { "AccountId", "RoleCode" },
                keyValues: new object[] { new Guid("11111111-1111-1111-1111-111111111111"), "IAM-ROLE-NV" });

            migrationBuilder.DeleteData(
                table: "iam_identity_account",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"));

            migrationBuilder.DropColumn(
                name: "C1ReviewNote",
                table: "lev_leave_request");

            migrationBuilder.DropColumn(
                name: "C1ReviewedAtUtc",
                table: "lev_leave_request");

            migrationBuilder.DropColumn(
                name: "C1ReviewedByIdpSubject",
                table: "lev_leave_request");

            migrationBuilder.UpdateData(
                table: "emp_employee",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                column: "LineManagerEmployeeId",
                value: null);
        }
    }
}
