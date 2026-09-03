using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Hrm.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPaySliceDAllowances : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "ContractAllowance",
                table: "pay_line",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MonthlyAllowance",
                table: "pay_line",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "pay_allowance_catalog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pay_allowance_catalog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "pay_contract_allowance",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeCode = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Code = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pay_contract_allowance", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "pay_monthly_allowance",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PeriodYm = table.Column<string>(type: "character varying(7)", maxLength: 7, nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeCode = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Code = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pay_monthly_allowance", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "pay_allowance_catalog",
                columns: new[] { "Id", "Code", "IsActive", "Name" },
                values: new object[,]
                {
                    { new Guid("c3c3c3c3-c3c3-c3c3-c3c3-c3c3c3c3c3c3"), "PC-ANTRUA", true, "Phụ cấp ăn trưa" },
                    { new Guid("c4c4c4c4-c4c4-c4c4-c4c4-c4c4c4c4c4c4"), "PC-XANG", true, "Phụ cấp xăng xe" }
                });

            migrationBuilder.InsertData(
                table: "pay_contract_allowance",
                columns: new[] { "Id", "Amount", "Code", "EmployeeCode", "EmployeeId" },
                values: new object[] { new Guid("c5c5c5c5-c5c5-c5c5-c5c5-c5c5c5c5c5c5"), 730000m, "PC-ANTRUA", "MNV-DEV", new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb") });

            migrationBuilder.CreateIndex(
                name: "IX_pay_allowance_catalog_Code",
                table: "pay_allowance_catalog",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_pay_contract_allowance_EmployeeId_Code",
                table: "pay_contract_allowance",
                columns: new[] { "EmployeeId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_pay_monthly_allowance_PeriodYm_EmployeeId_Code",
                table: "pay_monthly_allowance",
                columns: new[] { "PeriodYm", "EmployeeId", "Code" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "pay_allowance_catalog");

            migrationBuilder.DropTable(
                name: "pay_contract_allowance");

            migrationBuilder.DropTable(
                name: "pay_monthly_allowance");

            migrationBuilder.DropColumn(
                name: "ContractAllowance",
                table: "pay_line");

            migrationBuilder.DropColumn(
                name: "MonthlyAllowance",
                table: "pay_line");
        }
    }
}
