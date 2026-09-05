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
        // Raw SQL: InsertData cần model map đầy đủ (Designer); migration thủ công tránh crash startup.
        migrationBuilder.Sql(
            """
            INSERT INTO lev_leave_balance ("Id", "EmployeeId", "EntitledDays", "UsedDays", "Year")
            SELECT 'eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeef'::uuid,
                   'dddddddd-dddd-dddd-dddd-dddddddddddd'::uuid,
                   12.0, 0.0, 2026
            WHERE NOT EXISTS (
                SELECT 1 FROM lev_leave_balance
                WHERE "Id" = 'eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeef'::uuid
                   OR ("EmployeeId" = 'dddddddd-dddd-dddd-dddd-dddddddddddd'::uuid AND "Year" = 2026)
            );
            """);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(
            """
            DELETE FROM lev_leave_balance
            WHERE "Id" = 'eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeef'::uuid;
            """);
    }
}
