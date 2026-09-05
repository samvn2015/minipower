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
        // GTGC 11_000_000 cần precision rộng.
        builder.Property(x => x.DecimalValue).HasPrecision(18, 4);

        builder.HasData(
            new PayRegulation
            {
                Id = PaySeed.ProbationFactorId,
                Code = PayRegulationCodes.ProbationTimeWageFactor,
                Name = "Hệ số lương thời gian thử việc",
                DecimalValue = 0.85m
            },
            new PayRegulation
            {
                Id = PaySeed.StandardWorkDaysDefaultId,
                Code = PayRegulationCodes.StandardWorkDaysDefault,
                Name = "Ngày công chuẩn mặc định (C&B)",
                DecimalValue = 26m
            },
            new PayRegulation
            {
                Id = PaySeed.BhEmployeeRateId,
                Code = PayRegulationCodes.BhEmployeeRate,
                Name = "Tỷ lệ BH NLĐ tổng (legacy / hiển thị)",
                DecimalValue = 0.105m
            },
            new PayRegulation
            {
                Id = PaySeed.TncnTempRateId,
                Code = PayRegulationCodes.TncnTempRate,
                Name = "TNCN flat legacy (không dùng khi lũy tiến C&B)",
                DecimalValue = 0.05m
            },
            new PayRegulation
            {
                Id = PaySeed.BhxhRateId,
                Code = PayRegulationCodes.BhxhEmployeeRate,
                Name = "BHXH NLĐ (C&B)",
                DecimalValue = 0.08m
            },
            new PayRegulation
            {
                Id = PaySeed.BhytRateId,
                Code = PayRegulationCodes.BhytEmployeeRate,
                Name = "BHYT NLĐ (C&B)",
                DecimalValue = 0.015m
            },
            new PayRegulation
            {
                Id = PaySeed.BhtnRateId,
                Code = PayRegulationCodes.BhtnEmployeeRate,
                Name = "BHTN NLĐ (C&B)",
                DecimalValue = 0.01m
            },
            new PayRegulation
            {
                Id = PaySeed.TncnPersonalDeductionId,
                Code = PayRegulationCodes.TncnPersonalDeduction,
                Name = "Giảm trừ bản thân TNCN",
                DecimalValue = 11_000_000m
            },
            new PayRegulation
            {
                Id = PaySeed.TncnDependentUnitId,
                Code = PayRegulationCodes.TncnDependentUnit,
                Name = "Giảm trừ NPT / người",
                DecimalValue = 4_400_000m
            });
    }
}
