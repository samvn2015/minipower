using Hrm.Domain.Leave.Entities;
using Hrm.Infrastructure.Persistence.Emp;
using Hrm.Infrastructure.Persistence.Lev;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hrm.Infrastructure.Persistence.Configurations;

internal sealed class LeaveBalanceConfiguration : IEntityTypeConfiguration<LeaveBalance>
{
    public void Configure(EntityTypeBuilder<LeaveBalance> builder)
    {
        builder.ToTable("lev_leave_balance");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.EntitledDays).HasPrecision(5, 1);
        builder.Property(x => x.UsedDays).HasPrecision(5, 1);
        builder.Ignore(x => x.RemainingDays);
        builder.HasIndex(x => new { x.EmployeeId, x.Year }).IsUnique();

        builder.HasData(
            new LeaveBalance
            {
                Id = LevSeed.DevBalance2026Id,
                EmployeeId = EmpSeed.DevEmployeeId,
                Year = LevSeed.DevBalanceYear,
                EntitledDays = LevSeed.DevEntitledDays,
                UsedDays = 0m
            },
            new LeaveBalance
            {
                Id = LevSeed.HandoverBalance2026Id,
                EmployeeId = EmpSeed.HandoverEmployeeId,
                Year = LevSeed.DevBalanceYear,
                EntitledDays = LevSeed.DevEntitledDays,
                UsedDays = 0m
            });
    }
}
