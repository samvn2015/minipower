using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Hrm.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialIam : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "iam_identity_account",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IdpSubject = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    EmailCty = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    DisplayName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    EmployeeCode = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    Status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_iam_identity_account", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "iam_role",
                columns: table => new
                {
                    RoleCode = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_iam_role", x => x.RoleCode);
                });

            migrationBuilder.CreateTable(
                name: "iam_account_role",
                columns: table => new
                {
                    AccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    RoleCode = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_iam_account_role", x => new { x.AccountId, x.RoleCode });
                    table.ForeignKey(
                        name: "FK_iam_account_role_iam_identity_account_AccountId",
                        column: x => x.AccountId,
                        principalTable: "iam_identity_account",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_iam_account_role_iam_role_RoleCode",
                        column: x => x.RoleCode,
                        principalTable: "iam_role",
                        principalColumn: "RoleCode",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "iam_identity_account",
                columns: new[] { "Id", "DisplayName", "EmailCty", "EmployeeCode", "IdpSubject", "Status" },
                values: new object[] { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), "Dev IAM", "dev@company.local", "MNV-DEV", "local-dev", "Active" });

            migrationBuilder.InsertData(
                table: "iam_role",
                columns: new[] { "RoleCode", "Name" },
                values: new object[,]
                {
                    { "IAM-ROLE-HR", "HR / C&B" },
                    { "IAM-ROLE-IT", "IT Admin" },
                    { "IAM-ROLE-LM", "Line Manager" },
                    { "IAM-ROLE-NV", "Nhân viên" },
                    { "IAM-ROLE-PGD", "PGD / BGĐ" }
                });

            migrationBuilder.InsertData(
                table: "iam_account_role",
                columns: new[] { "AccountId", "RoleCode" },
                values: new object[,]
                {
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), "IAM-ROLE-HR" },
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), "IAM-ROLE-NV" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_iam_account_role_RoleCode",
                table: "iam_account_role",
                column: "RoleCode");

            migrationBuilder.CreateIndex(
                name: "IX_iam_identity_account_IdpSubject",
                table: "iam_identity_account",
                column: "IdpSubject",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "iam_account_role");

            migrationBuilder.DropTable(
                name: "iam_identity_account");

            migrationBuilder.DropTable(
                name: "iam_role");
        }
    }
}
