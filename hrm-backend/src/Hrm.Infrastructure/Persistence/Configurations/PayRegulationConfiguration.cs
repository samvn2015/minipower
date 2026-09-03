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
        // 0.85 (TV) và 22 (ngày công chuẩn) cùng cột — cần precision rộng hơn (5,4).
        builder.Property(x => x.DecimalValue).HasPrecision(8, 4);

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
                Name = "Ngày công chuẩn mặc định (khi tháng chưa có lịch)",
                DecimalValue = 22m
            },
            new PayRegulation
            {
                Id = PaySeed.BhEmployeeRateId,
                Code = PayRegulationCodes.BhEmployeeRate,
                Name = "Tỷ lệ BH người lao động (hiệu lực kỳ)",
                DecimalValue = 0.10m
            },
            new PayRegulation
            {
                Id = PaySeed.TncnTempRateId,
                Code = PayRegulationCodes.TncnTempRate,
                Name = "Tỷ lệ TNCN tạm (hiệu lực kỳ)",
                DecimalValue = 0.05m
            });
    }
}
