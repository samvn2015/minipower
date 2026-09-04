using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Hrm.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPaySliceKCbQuyChe : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "DecimalValue",
                table: "pay_regulation",
                type: "numeric(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(8,4)",
                oldPrecision: 8,
                oldScale: 4);

            migrationBuilder.AddColumn<int>(
                name: "DependentCount",
                table: "pay_contract_salary",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.InsertData(
                table: "pay_allowance_catalog",
                columns: new[] { "Id", "Code", "IsActive", "Name" },
                values: new object[,]
                {
                    { new Guid("d1d1d1d1-d1d1-d1d1-d1d1-d1d1d1d1d1d1"), "PC-TRACHNHIEM", true, "Phụ cấp trách nhiệm" },
                    { new Guid("d2d2d2d2-d2d2-d2d2-d2d2-d2d2d2d2d2d2"), "PC-DIENTHOAI", true, "Phụ cấp điện thoại" },
                    { new Guid("d3d3d3d3-d3d3-d3d3-d3d3-d3d3d3d3d3d3"), "PC-TAMUNG", true, "Tạm ứng (trừ thực lĩnh)" }
                });

            migrationBuilder.UpdateData(
                table: "pay_contract_salary",
                keyColumn: "Id",
                keyValue: new Guid("c8c8c8c8-c8c8-c8c8-c8c8-c8c8c8c8c8c8"),
                column: "DependentCount",
                value: 0);

            migrationBuilder.UpdateData(
                table: "pay_regulation",
                keyColumn: "Id",
                keyValue: new Guid("c2c2c2c2-c2c2-c2c2-c2c2-c2c2c2c2c2c2"),
                columns: new[] { "DecimalValue", "Name" },
                values: new object[] { 26m, "Ngày công chuẩn mặc định (C&B)" });

            migrationBuilder.UpdateData(
                table: "pay_regulation",
                keyColumn: "Id",
                keyValue: new Guid("c6c6c6c6-c6c6-c6c6-c6c6-c6c6c6c6c6c6"),
                columns: new[] { "DecimalValue", "Name" },
                values: new object[] { 0.105m, "Tỷ lệ BH NLĐ tổng (legacy / hiển thị)" });

            migrationBuilder.UpdateData(
                table: "pay_regulation",
                keyColumn: "Id",
                keyValue: new Guid("c7c7c7c7-c7c7-c7c7-c7c7-c7c7c7c7c7c7"),
                column: "Name",
                value: "TNCN flat legacy (không dùng khi lũy tiến C&B)");

            migrationBuilder.InsertData(
                table: "pay_regulation",
                columns: new[] { "Id", "Code", "DecimalValue", "Name" },
                values: new object[,]
                {
                    { new Guid("d4d4d4d4-d4d4-d4d4-d4d4-d4d4d4d4d4d4"), "BHXH_EMPLOYEE_RATE", 0.08m, "BHXH NLĐ (C&B)" },
                    { new Guid("d5d5d5d5-d5d5-d5d5-d5d5-d5d5d5d5d5d5"), "BHYT_EMPLOYEE_RATE", 0.015m, "BHYT NLĐ (C&B)" },
                    { new Guid("d6d6d6d6-d6d6-d6d6-d6d6-d6d6d6d6d6d6"), "BHTN_EMPLOYEE_RATE", 0.01m, "BHTN NLĐ (C&B)" },
                    { new Guid("d7d7d7d7-d7d7-d7d7-d7d7-d7d7d7d7d7d7"), "TNCN_PERSONAL_DEDUCTION", 11000000m, "Giảm trừ bản thân TNCN" },
                    { new Guid("d8d8d8d8-d8d8-d8d8-d8d8-d8d8d8d8d8d8"), "TNCN_DEPENDENT_UNIT", 4400000m, "Giảm trừ NPT / người" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "pay_allowance_catalog",
                keyColumn: "Id",
                keyValue: new Guid("d1d1d1d1-d1d1-d1d1-d1d1-d1d1d1d1d1d1"));

            migrationBuilder.DeleteData(
                table: "pay_allowance_catalog",
                keyColumn: "Id",
                keyValue: new Guid("d2d2d2d2-d2d2-d2d2-d2d2-d2d2d2d2d2d2"));

            migrationBuilder.DeleteData(
                table: "pay_allowance_catalog",
                keyColumn: "Id",
                keyValue: new Guid("d3d3d3d3-d3d3-d3d3-d3d3-d3d3d3d3d3d3"));

            migrationBuilder.DeleteData(
                table: "pay_regulation",
                keyColumn: "Id",
                keyValue: new Guid("d4d4d4d4-d4d4-d4d4-d4d4-d4d4d4d4d4d4"));

            migrationBuilder.DeleteData(
                table: "pay_regulation",
                keyColumn: "Id",
                keyValue: new Guid("d5d5d5d5-d5d5-d5d5-d5d5-d5d5d5d5d5d5"));

            migrationBuilder.DeleteData(
                table: "pay_regulation",
                keyColumn: "Id",
                keyValue: new Guid("d6d6d6d6-d6d6-d6d6-d6d6-d6d6d6d6d6d6"));

            migrationBuilder.DeleteData(
                table: "pay_regulation",
                keyColumn: "Id",
                keyValue: new Guid("d7d7d7d7-d7d7-d7d7-d7d7-d7d7d7d7d7d7"));

            migrationBuilder.DeleteData(
                table: "pay_regulation",
                keyColumn: "Id",
                keyValue: new Guid("d8d8d8d8-d8d8-d8d8-d8d8-d8d8d8d8d8d8"));

            migrationBuilder.DropColumn(
                name: "DependentCount",
                table: "pay_contract_salary");

            migrationBuilder.AlterColumn<decimal>(
                name: "DecimalValue",
                table: "pay_regulation",
                type: "numeric(8,4)",
                precision: 8,
                scale: 4,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,4)",
                oldPrecision: 18,
                oldScale: 4);

            migrationBuilder.UpdateData(
                table: "pay_regulation",
                keyColumn: "Id",
                keyValue: new Guid("c2c2c2c2-c2c2-c2c2-c2c2-c2c2c2c2c2c2"),
                columns: new[] { "DecimalValue", "Name" },
                values: new object[] { 22m, "Ngày công chuẩn mặc định (khi tháng chưa có lịch)" });

            migrationBuilder.UpdateData(
                table: "pay_regulation",
                keyColumn: "Id",
                keyValue: new Guid("c6c6c6c6-c6c6-c6c6-c6c6-c6c6c6c6c6c6"),
                columns: new[] { "DecimalValue", "Name" },
                values: new object[] { 0.10m, "Tỷ lệ BH người lao động (hiệu lực kỳ)" });

            migrationBuilder.UpdateData(
                table: "pay_regulation",
                keyColumn: "Id",
                keyValue: new Guid("c7c7c7c7-c7c7-c7c7-c7c7-c7c7c7c7c7c7"),
                column: "Name",
                value: "Tỷ lệ TNCN tạm (hiệu lực kỳ)");
        }
    }
}
