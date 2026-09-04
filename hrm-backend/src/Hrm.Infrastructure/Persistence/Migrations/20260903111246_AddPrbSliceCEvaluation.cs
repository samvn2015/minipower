using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Hrm.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPrbSliceCEvaluation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "prb_criterion",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_prb_criterion", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "prb_evaluation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeCode = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    ProbationEndDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Status = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    ProposedOutcomeCode = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    ProposedByIdpSubject = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ProposedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ProposedNote = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    CriteriaPayloadJson = table.Column<string>(type: "character varying(8000)", maxLength: 8000, nullable: true),
                    DecidedOutcomeCode = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    DecidedByIdpSubject = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    DecidedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DecisionNote = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    ExtendDurationCode = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_prb_evaluation", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "prb_extend_duration",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    Name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Months = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_prb_extend_duration", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "prb_outcome",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    Name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_prb_outcome", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "prb_criterion",
                columns: new[] { "Id", "Code", "IsActive", "Name", "SortOrder" },
                values: new object[,]
                {
                    { new Guid("d4d4d4d4-d4d4-d4d4-d4d4-d4d4d4d4d4d4"), "CRIT-WORK", true, "Kết quả công việc", 1 },
                    { new Guid("d5d5d5d5-d5d5-d5d5-d5d5-d5d5d5d5d5d5"), "CRIT-ATTITUDE", true, "Thái độ / kỷ luật", 2 }
                });

            migrationBuilder.InsertData(
                table: "prb_extend_duration",
                columns: new[] { "Id", "Code", "IsActive", "Months", "Name", "SortOrder" },
                values: new object[,]
                {
                    { new Guid("d6d6d6d6-d6d6-d6d6-d6d6-d6d6d6d6d6d6"), "EXT-1M", true, 1, "Gia hạn 1 tháng", 1 },
                    { new Guid("d7d7d7d7-d7d7-d7d7-d7d7-d7d7d7d7d7d7"), "EXT-2M", true, 2, "Gia hạn 2 tháng", 2 }
                });

            migrationBuilder.InsertData(
                table: "prb_outcome",
                columns: new[] { "Id", "Code", "IsActive", "Name", "SortOrder" },
                values: new object[,]
                {
                    { new Guid("d1d1d1d1-d1d1-d1d1-d1d1-d1d1d1d1d1d1"), "PASS", true, "Đạt", 1 },
                    { new Guid("d2d2d2d2-d2d2-d2d2-d2d2-d2d2d2d2d2d2"), "EXTEND", true, "Gia hạn", 2 },
                    { new Guid("d3d3d3d3-d3d3-d3d3-d3d3-d3d3d3d3d3d3"), "FAIL", true, "Không đạt", 3 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_prb_criterion_Code",
                table: "prb_criterion",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_prb_evaluation_EmployeeCode",
                table: "prb_evaluation",
                column: "EmployeeCode");

            migrationBuilder.CreateIndex(
                name: "IX_prb_evaluation_EmployeeId_Status",
                table: "prb_evaluation",
                columns: new[] { "EmployeeId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_prb_extend_duration_Code",
                table: "prb_extend_duration",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_prb_outcome_Code",
                table: "prb_outcome",
                column: "Code",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "prb_criterion");

            migrationBuilder.DropTable(
                name: "prb_evaluation");

            migrationBuilder.DropTable(
                name: "prb_extend_duration");

            migrationBuilder.DropTable(
                name: "prb_outcome");
        }
    }
}
