using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hrm.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddLifSliceCNPlus3Locks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CrmSpLockedAtUtc",
                table: "lif_offboarding_case",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EarlyCrReason",
                table: "lif_offboarding_case",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "GitLockedAtUtc",
                table: "lif_offboarding_case",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsEarlySecurityCr",
                table: "lif_offboarding_case",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateOnly>(
                name: "LockAsOfDate",
                table: "lif_offboarding_case",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LockedByIdpSubject",
                table: "lif_offboarding_case",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "lif_access_lock_outbox",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CaseId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeCode = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    TargetSystems = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Channel = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    AsOfDate = table.Column<DateOnly>(type: "date", nullable: false),
                    IsEarlySecurityCr = table.Column<bool>(type: "boolean", nullable: false),
                    CrReason = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedByIdpSubject = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_lif_access_lock_outbox", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_lif_access_lock_outbox_AsOfDate",
                table: "lif_access_lock_outbox",
                column: "AsOfDate");

            migrationBuilder.CreateIndex(
                name: "IX_lif_access_lock_outbox_CaseId",
                table: "lif_access_lock_outbox",
                column: "CaseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "lif_access_lock_outbox");

            migrationBuilder.DropColumn(
                name: "CrmSpLockedAtUtc",
                table: "lif_offboarding_case");

            migrationBuilder.DropColumn(
                name: "EarlyCrReason",
                table: "lif_offboarding_case");

            migrationBuilder.DropColumn(
                name: "GitLockedAtUtc",
                table: "lif_offboarding_case");

            migrationBuilder.DropColumn(
                name: "IsEarlySecurityCr",
                table: "lif_offboarding_case");

            migrationBuilder.DropColumn(
                name: "LockAsOfDate",
                table: "lif_offboarding_case");

            migrationBuilder.DropColumn(
                name: "LockedByIdpSubject",
                table: "lif_offboarding_case");
        }
    }
}
