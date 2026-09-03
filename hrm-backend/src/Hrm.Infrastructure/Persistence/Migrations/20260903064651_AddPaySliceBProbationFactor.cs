using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hrm.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPaySliceBProbationFactor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "TimeWageFactor",
                table: "pay_line",
                type: "numeric(5,4)",
                precision: 5,
                scale: 4,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "pay_regulation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    DecimalValue = table.Column<decimal>(type: "numeric(5,4)", precision: 5, scale: 4, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pay_regulation", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "pay_regulation",
                columns: new[] { "Id", "Code", "DecimalValue", "Name" },
                values: new object[] { new Guid("c1c1c1c1-c1c1-c1c1-c1c1-c1c1c1c1c1c1"), "PROBATION_TIME_WAGE_FACTOR", 0.85m, "Hệ số lương thời gian thử việc" });

            migrationBuilder.CreateIndex(
                name: "IX_pay_regulation_Code",
                table: "pay_regulation",
                column: "Code",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "pay_regulation");

            migrationBuilder.DropColumn(
                name: "TimeWageFactor",
                table: "pay_line");
        }
    }
}
