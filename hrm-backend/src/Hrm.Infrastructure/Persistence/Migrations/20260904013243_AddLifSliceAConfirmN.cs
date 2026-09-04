using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hrm.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddLifSliceAConfirmN : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ConfirmedAtUtc",
                table: "lif_offboarding_case",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ConfirmedByIdpSubject",
                table: "lif_offboarding_case",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "ResignationSignedDate",
                table: "lif_offboarding_case",
                type: "date",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConfirmedAtUtc",
                table: "lif_offboarding_case");

            migrationBuilder.DropColumn(
                name: "ConfirmedByIdpSubject",
                table: "lif_offboarding_case");

            migrationBuilder.DropColumn(
                name: "ResignationSignedDate",
                table: "lif_offboarding_case");
        }
    }
}
