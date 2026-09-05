using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hrm.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTimSliceCPeriodClose : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "OtUnclassified",
                table: "tim_timesheet_line",
                type: "numeric(5,2)",
                precision: 5,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "ClosedAtUtc",
                table: "tim_period",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClosedByIdpSubject",
                table: "tim_period",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "OtUnclassified",
                table: "tim_import_row",
                type: "numeric(5,2)",
                precision: 5,
                scale: 2,
                nullable: true);

            migrationBuilder.InsertData(
                table: "tim_template_column",
                columns: new[] { "Id", "ColumnKey", "DisplayName", "IsRequired", "MapsTo", "SortOrder", "TemplateVersionId" },
                values: new object[] { new Guid("b6b6b6b6-b6b6-b6b6-b6b6-b6b6b6b6b6b6"), "ot_unclassified", "OT chưa phân loại", false, "OtUnclassified", 6, new Guid("a1a1a1a1-a1a1-a1a1-a1a1-a1a1a1a1a1a1") });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "tim_template_column",
                keyColumn: "Id",
                keyValue: new Guid("b6b6b6b6-b6b6-b6b6-b6b6-b6b6b6b6b6b6"));

            migrationBuilder.DropColumn(
                name: "OtUnclassified",
                table: "tim_timesheet_line");

            migrationBuilder.DropColumn(
                name: "ClosedAtUtc",
                table: "tim_period");

            migrationBuilder.DropColumn(
                name: "ClosedByIdpSubject",
                table: "tim_period");

            migrationBuilder.DropColumn(
                name: "OtUnclassified",
                table: "tim_import_row");
        }
    }
}
