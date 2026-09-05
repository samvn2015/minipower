using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hrm.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPaySliceCWorkdayCap : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "DecimalValue",
                table: "pay_regulation",
                type: "numeric(8,4)",
                precision: 8,
                scale: 4,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(5,4)",
                oldPrecision: 5,
                oldScale: 4);

            migrationBuilder.CreateTable(
                name: "pay_workday_calendar",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PeriodYm = table.Column<string>(type: "character varying(7)", maxLength: 7, nullable: false),
                    StandardWorkDays = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pay_workday_calendar", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "pay_regulation",
                columns: new[] { "Id", "Code", "DecimalValue", "Name" },
                values: new object[] { new Guid("c2c2c2c2-c2c2-c2c2-c2c2-c2c2c2c2c2c2"), "STANDARD_WORK_DAYS_DEFAULT", 22m, "Ngày công chuẩn mặc định (khi tháng chưa có lịch)" });

            migrationBuilder.CreateIndex(
                name: "IX_pay_workday_calendar_PeriodYm",
                table: "pay_workday_calendar",
                column: "PeriodYm",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "pay_workday_calendar");

            migrationBuilder.DeleteData(
                table: "pay_regulation",
                keyColumn: "Id",
                keyValue: new Guid("c2c2c2c2-c2c2-c2c2-c2c2-c2c2c2c2c2c2"));

            migrationBuilder.AlterColumn<decimal>(
                name: "DecimalValue",
                table: "pay_regulation",
                type: "numeric(5,4)",
                precision: 5,
                scale: 4,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(8,4)",
                oldPrecision: 8,
                oldScale: 4);
        }
    }
}
