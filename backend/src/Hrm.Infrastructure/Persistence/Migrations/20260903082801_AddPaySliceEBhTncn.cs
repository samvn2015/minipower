using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Hrm.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPaySliceEBhTncn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "BhAmount",
                table: "pay_line",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "BhRate",
                table: "pay_line",
                type: "numeric(8,4)",
                precision: 8,
                scale: 4,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "NetPay",
                table: "pay_line",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TncnAmount",
                table: "pay_line",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TncnRate",
                table: "pay_line",
                type: "numeric(8,4)",
                precision: 8,
                scale: 4,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "pay_contract_salary",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeCode = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pay_contract_salary", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "pay_contract_salary",
                columns: new[] { "Id", "Amount", "EmployeeCode", "EmployeeId" },
                values: new object[] { new Guid("c8c8c8c8-c8c8-c8c8-c8c8-c8c8c8c8c8c8"), 10000000m, "MNV-DEV", new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb") });

            migrationBuilder.InsertData(
                table: "pay_regulation",
                columns: new[] { "Id", "Code", "DecimalValue", "Name" },
                values: new object[,]
                {
                    { new Guid("c6c6c6c6-c6c6-c6c6-c6c6-c6c6c6c6c6c6"), "BH_EMPLOYEE_RATE", 0.10m, "Tỷ lệ BH người lao động (hiệu lực kỳ)" },
                    { new Guid("c7c7c7c7-c7c7-c7c7-c7c7-c7c7c7c7c7c7"), "TNCN_TEMP_RATE", 0.05m, "Tỷ lệ TNCN tạm (hiệu lực kỳ)" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_pay_contract_salary_EmployeeId",
                table: "pay_contract_salary",
                column: "EmployeeId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "pay_contract_salary");

            migrationBuilder.DeleteData(
                table: "pay_regulation",
                keyColumn: "Id",
                keyValue: new Guid("c6c6c6c6-c6c6-c6c6-c6c6-c6c6c6c6c6c6"));

            migrationBuilder.DeleteData(
                table: "pay_regulation",
                keyColumn: "Id",
                keyValue: new Guid("c7c7c7c7-c7c7-c7c7-c7c7-c7c7c7c7c7c7"));

            migrationBuilder.DropColumn(
                name: "BhAmount",
                table: "pay_line");

            migrationBuilder.DropColumn(
                name: "BhRate",
                table: "pay_line");

            migrationBuilder.DropColumn(
                name: "NetPay",
                table: "pay_line");

            migrationBuilder.DropColumn(
                name: "TncnAmount",
                table: "pay_line");

            migrationBuilder.DropColumn(
                name: "TncnRate",
                table: "pay_line");
        }
    }
}
