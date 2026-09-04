using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Hrm.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddLifSliceDOnboarding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "lif_on_checklist_item",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    IsMust = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_lif_on_checklist_item", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "lif_on_checklist_tick",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OnboardingCaseId = table.Column<Guid>(type: "uuid", nullable: false),
                    ItemCode = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    IsChecked = table.Column<bool>(type: "boolean", nullable: false),
                    CheckedByIdpSubject = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    CheckedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_lif_on_checklist_tick", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "lif_onboarding_case",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeCode = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Status = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedByIdpSubject = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Note = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    EmailCtyProvisioned = table.Column<bool>(type: "boolean", nullable: false),
                    GitProvisioned = table.Column<bool>(type: "boolean", nullable: false),
                    CrmSpProvisioned = table.Column<bool>(type: "boolean", nullable: false),
                    ChatProvisioned = table.Column<bool>(type: "boolean", nullable: false),
                    EmailCtyProvisionedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    GitProvisionedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CrmSpProvisionedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ChatProvisionedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ClosedByIdpSubject = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ClosedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_lif_onboarding_case", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "lif_on_checklist_item",
                columns: new[] { "Id", "Code", "IsActive", "IsMust", "Name", "SortOrder" },
                values: new object[,]
                {
                    { new Guid("a1a1a1a1-a1a1-a1a1-a1a1-a1a1a1a1a1a1"), "ON-PAPERWORK", true, true, "Hồ sơ / giấy tờ nhận việc", 1 },
                    { new Guid("a2a2a2a2-a2a2-a2a2-a2a2-a2a2a2a2a2a2"), "ON-ORIENTATION", true, true, "Orientation nội bộ", 2 },
                    { new Guid("a3a3a3a3-a3a3-a3a3-a3a3-a3a3a3a3a3a3"), "ON-BUDDY", true, false, "Gán buddy", 3 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_lif_on_checklist_item_Code",
                table: "lif_on_checklist_item",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_lif_on_checklist_tick_OnboardingCaseId_ItemCode",
                table: "lif_on_checklist_tick",
                columns: new[] { "OnboardingCaseId", "ItemCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_lif_onboarding_case_EmployeeId",
                table: "lif_onboarding_case",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_lif_onboarding_case_Status",
                table: "lif_onboarding_case",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "lif_on_checklist_item");

            migrationBuilder.DropTable(
                name: "lif_on_checklist_tick");

            migrationBuilder.DropTable(
                name: "lif_onboarding_case");
        }
    }
}
