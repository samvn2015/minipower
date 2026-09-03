using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hrm.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPrbSliceBReminders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "prb_reminder",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Kind = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeCode = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    ProbationEndDate = table.Column<DateOnly>(type: "date", nullable: false),
                    DueDate = table.Column<DateOnly>(type: "date", nullable: false),
                    AsOfDate = table.Column<DateOnly>(type: "date", nullable: false),
                    AssigneeEmployeeId = table.Column<Guid>(type: "uuid", nullable: true),
                    AssigneeEmployeeCode = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    InAppMessage = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false),
                    EmailTo = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    Channel = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedByIdpSubject = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_prb_reminder", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_prb_reminder_AsOfDate",
                table: "prb_reminder",
                column: "AsOfDate");

            migrationBuilder.CreateIndex(
                name: "IX_prb_reminder_EmployeeId_Kind_ProbationEndDate",
                table: "prb_reminder",
                columns: new[] { "EmployeeId", "Kind", "ProbationEndDate" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "prb_reminder");
        }
    }
}
