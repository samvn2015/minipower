using Hrm.Domain.Leave;
using Hrm.Domain.Leave.Entities;
using Hrm.Infrastructure.Persistence.Lev;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hrm.Infrastructure.Persistence.Configurations;

internal sealed class LeaveTypeConfiguration : IEntityTypeConfiguration<LeaveType>
{
    public void Configure(EntityTypeBuilder<LeaveType> builder)
    {
        builder.ToTable("lev_leave_type");
        builder.HasKey(x => x.Code);
        builder.Property(x => x.Code).HasMaxLength(32);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(128);
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(32);

        builder.HasData(
            new LeaveType
            {
                Code = LevSeed.AnnualCode,
                Name = "Phép năm",
                DeductsAnnualBalance = true,
                Status = LeaveTypeStatus.Active
            },
            new LeaveType
            {
                Code = LevSeed.UnpaidCode,
                Name = "Phép không hưởng lương",
                DeductsAnnualBalance = false,
                Status = LeaveTypeStatus.Active
            },
            new LeaveType
            {
                Code = LevSeed.SickCode,
                Name = "Phép ốm/BHXH",
                DeductsAnnualBalance = false,
                Status = LeaveTypeStatus.Active
            },
            new LeaveType
            {
                Code = LevSeed.MarriageCode,
                Name = "Phép kết hôn",
                DeductsAnnualBalance = false,
                Status = LeaveTypeStatus.Active
            },
            new LeaveType
            {
                Code = LevSeed.BereavementCode,
                Name = "Phép tang chế",
                DeductsAnnualBalance = false,
                Status = LeaveTypeStatus.Active
            },
            new LeaveType
            {
                Code = LevSeed.MaternityCode,
                Name = "Nghỉ chế độ Nam/Nữ",
                DeductsAnnualBalance = false,
                Status = LeaveTypeStatus.Active
            });
    }
}
