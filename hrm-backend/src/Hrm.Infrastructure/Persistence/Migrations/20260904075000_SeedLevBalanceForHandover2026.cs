using System;
using Hrm.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hrm.Infrastructure.Persistence.Migrations;

/// <summary>Seed quỹ phép 2026 cho MNV-HO (local-lm) — UAT LEV không 40401.</summary>
[DbContext(typeof(AppDbContext))]
[Migration("20260904075000_SeedLevBalanceForHandover2026")]
public class SeedLevBalanceForHandover2026 : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.InsertData(
            table: "lev_leave_balance",
            columns: new[] { "Id", "EmployeeId", "EntitledDays", "UsedDays", "Year" },
            values: new object[]
            {
                new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeef"),
                new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"),
                12m,
                0m,
                2026
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DeleteData(
            table: "lev_leave_balance",
            keyColumn: "Id",
            keyValue: new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeef"));
    }
}
