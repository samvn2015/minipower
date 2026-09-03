using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Hrm.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTimSliceATemplate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tim_template_version",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    VersionCode = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    PublishedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    PublishedByIdpSubject = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tim_template_version", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tim_template_column",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TemplateVersionId = table.Column<Guid>(type: "uuid", nullable: false),
                    ColumnKey = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    IsRequired = table.Column<bool>(type: "boolean", nullable: false),
                    MapsTo = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tim_template_column", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tim_template_column_tim_template_version_TemplateVersionId",
                        column: x => x.TemplateVersionId,
                        principalTable: "tim_template_version",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "tim_template_version",
                columns: new[] { "Id", "Name", "PublishedAtUtc", "PublishedByIdpSubject", "Status", "VersionCode" },
                values: new object[] { new Guid("a1a1a1a1-a1a1-a1a1-a1a1-a1a1a1a1a1a1"), "Mẫu công V1 (seed)", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed", "Active", "TIM-V1" });

            migrationBuilder.InsertData(
                table: "tim_template_column",
                columns: new[] { "Id", "ColumnKey", "DisplayName", "IsRequired", "MapsTo", "SortOrder", "TemplateVersionId" },
                values: new object[,]
                {
                    { new Guid("b1b1b1b1-b1b1-b1b1-b1b1-b1b1b1b1b1b1"), "mnv", "Mã NV", true, "EmployeeCode", 1, new Guid("a1a1a1a1-a1a1-a1a1-a1a1-a1a1a1a1a1a1") },
                    { new Guid("b2b2b2b2-b2b2-b2b2-b2b2-b2b2b2b2b2b2"), "n_thuc", "Ngày công thực", true, "WorkDays", 2, new Guid("a1a1a1a1-a1a1-a1a1-a1a1-a1a1a1a1a1a1") },
                    { new Guid("b3b3b3b3-b3b3-b3b3-b3b3-b3b3b3b3b3b3"), "ot_15", "OT 1.5", false, "Ot15", 3, new Guid("a1a1a1a1-a1a1-a1a1-a1a1-a1a1a1a1a1a1") },
                    { new Guid("b4b4b4b4-b4b4-b4b4-b4b4-b4b4b4b4b4b4"), "ot_20", "OT 2.0", false, "Ot20", 4, new Guid("a1a1a1a1-a1a1-a1a1-a1a1-a1a1a1a1a1a1") },
                    { new Guid("b5b5b5b5-b5b5-b5b5-b5b5-b5b5b5b5b5b5"), "ot_30", "OT 3.0", false, "Ot30", 5, new Guid("a1a1a1a1-a1a1-a1a1-a1a1-a1a1a1a1a1a1") }
                });

            migrationBuilder.CreateIndex(
                name: "IX_tim_template_column_TemplateVersionId_ColumnKey",
                table: "tim_template_column",
                columns: new[] { "TemplateVersionId", "ColumnKey" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tim_template_version_Status",
                table: "tim_template_version",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_tim_template_version_VersionCode",
                table: "tim_template_version",
                column: "VersionCode",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tim_template_column");

            migrationBuilder.DropTable(
                name: "tim_template_version");
        }
    }
}
