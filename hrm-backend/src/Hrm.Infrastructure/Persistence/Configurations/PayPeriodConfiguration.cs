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
        builder.Property(x => x.ClosedByIdpSubject).HasMaxLength(256);
    }
}
