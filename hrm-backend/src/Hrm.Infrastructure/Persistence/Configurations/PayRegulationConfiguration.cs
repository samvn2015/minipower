using Hrm.Domain.Payroll;
using Hrm.Domain.Payroll.Entities;
using Hrm.Infrastructure.Persistence.Pay;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hrm.Infrastructure.Persistence.Configurations;

internal sealed class PayRegulationConfiguration : IEntityTypeConfiguration<PayRegulation>
{
    public void Configure(EntityTypeBuilder<PayRegulation> builder)
    {
        builder.ToTable("pay_regulation");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Code).IsRequired().HasMaxLength(64);
        builder.HasIndex(x => x.Code).IsUnique();
        builder.Property(x => x.Name).IsRequired().HasMaxLength(256);
        builder.Property(x => x.DecimalValue).HasPrecision(5, 4);

        builder.HasData(new PayRegulation
        {
            Id = PaySeed.ProbationFactorId,
            Code = PayRegulationCodes.ProbationTimeWageFactor,
            Name = "Hệ số lương thời gian thử việc",
            DecimalValue = 0.85m
        });
    }
}
