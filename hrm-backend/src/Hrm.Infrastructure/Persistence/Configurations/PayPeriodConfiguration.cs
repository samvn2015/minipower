using Hrm.Domain.Payroll;
using Hrm.Domain.Payroll.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hrm.Infrastructure.Persistence.Configurations;

internal sealed class PayPeriodConfiguration : IEntityTypeConfiguration<PayPeriod>
{
    public void Configure(EntityTypeBuilder<PayPeriod> builder)
    {
        builder.ToTable("pay_period");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.PeriodYm).IsRequired().HasMaxLength(7);
        builder.HasIndex(x => x.PeriodYm).IsUnique();
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(32);
        builder.Property(x => x.RanByIdpSubject).HasMaxLength(256);
        builder.Property(x => x.ClosedByIdpSubject).HasMaxLength(256);
    }
}

internal sealed class PayLineConfiguration : IEntityTypeConfiguration<PayLine>
{
    public void Configure(EntityTypeBuilder<PayLine> builder)
    {
        builder.ToTable("pay_line");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.EmployeeCode).IsRequired().HasMaxLength(64);
        builder.Property(x => x.WorkDays).HasPrecision(5, 2);
        builder.Property(x => x.LeaveDaysUnpaid).HasPrecision(5, 2);
        builder.Property(x => x.LeaveDaysPaid).HasPrecision(5, 2);
        builder.Property(x => x.NTinh).HasPrecision(5, 2);
        builder.Property(x => x.TimeWageFactor).HasPrecision(5, 4);
        builder.Property(x => x.Ot15).HasPrecision(5, 2);
        builder.Property(x => x.Ot20).HasPrecision(5, 2);
        builder.Property(x => x.Ot30).HasPrecision(5, 2);
        builder.HasOne(x => x.Period)
            .WithMany(x => x.Lines)
            .HasForeignKey(x => x.PeriodId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasIndex(x => new { x.PeriodId, x.EmployeeId }).IsUnique();
    }
}
