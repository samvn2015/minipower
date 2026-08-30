using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hrm.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddLevSliceC2Review : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "C2ReviewNote",
                table: "lev_leave_request",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "C2ReviewedAtUtc",
                table: "lev_leave_request",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "C2ReviewedByIdpSubject",
                table: "lev_leave_request",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "C2ReviewNote",
                table: "lev_leave_request");

            migrationBuilder.DropColumn(
                name: "C2ReviewedAtUtc",
                table: "lev_leave_request");

            migrationBuilder.DropColumn(
                name: "C2ReviewedByIdpSubject",
                table: "lev_leave_request");
        }
    }
}
