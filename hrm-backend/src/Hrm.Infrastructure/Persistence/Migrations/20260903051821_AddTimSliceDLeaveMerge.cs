using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hrm.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTimSliceDLeaveMerge : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "LeaveDaysOther",
                table: "tim_timesheet_line",
                type: "numeric(5,2)",
                precision: 5,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "LeaveDaysPaid",
                table: "tim_timesheet_line",
                type: "numeric(5,2)",
                precision: 5,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "LeaveDaysUnpaid",
                table: "tim_timesheet_line",
                type: "numeric(5,2)",
                precision: 5,
                scale: 2,
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LeaveDaysOther",
                table: "tim_timesheet_line");

            migrationBuilder.DropColumn(
                name: "LeaveDaysPaid",
                table: "tim_timesheet_line");

            migrationBuilder.DropColumn(
                name: "LeaveDaysUnpaid",
                table: "tim_timesheet_line");
        }
    }
}
