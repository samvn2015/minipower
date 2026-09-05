using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Hrm.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddLifSliceBOffChecklist : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "lif_off_checklist_item",
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
                    table.PrimaryKey("PK_lif_off_checklist_item", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "lif_off_checklist_tick",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OffboardingCaseId = table.Column<Guid>(type: "uuid", nullable: false),
                    ItemCode = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    IsChecked = table.Column<bool>(type: "boolean", nullable: false),
                    CheckedByIdpSubject = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    CheckedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_lif_off_checklist_tick", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "lif_off_checklist_item",
                columns: new[] { "Id", "Code", "IsActive", "IsMust", "Name", "SortOrder" },
                values: new object[,]
                {
                    { new Guid("e1e1e1e1-e1e1-e1e1-e1e1-e1e1e1e1e1e1"), "OFF-RETURN-LAPTOP", true, true, "Thu hồi laptop / thiết bị", 1 },
                    { new Guid("e2e2e2e2-e2e2-e2e2-e2e2-e2e2e2e2e2e2"), "OFF-RETURN-BADGE", true, true, "Thu hồi thẻ ra vào", 2 },
                    { new Guid("e3e3e3e3-e3e3-e3e3-e3e3-e3e3e3e3e3e3"), "OFF-HANDOVER", true, true, "Bàn giao công việc / tài liệu", 3 },
                    { new Guid("e4e4e4e4-e4e4-e4e4-e4e4-e4e4e4e4e4e4"), "OFF-EXIT-INTERVIEW", true, false, "Phỏng vấn nghỉ việc", 4 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_lif_off_checklist_item_Code",
                table: "lif_off_checklist_item",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_lif_off_checklist_tick_OffboardingCaseId_ItemCode",
                table: "lif_off_checklist_tick",
                columns: new[] { "OffboardingCaseId", "ItemCode" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "lif_off_checklist_item");

            migrationBuilder.DropTable(
                name: "lif_off_checklist_tick");
        }
    }
}
