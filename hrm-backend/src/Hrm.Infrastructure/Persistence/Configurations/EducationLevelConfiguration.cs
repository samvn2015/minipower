using Hrm.Domain.Employees;
using Hrm.Domain.Employees.Entities;
using Hrm.Infrastructure.Persistence.Emp;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hrm.Infrastructure.Persistence.Configurations;

internal sealed class EducationLevelConfiguration : IEntityTypeConfiguration<EducationLevel>
{
    public void Configure(EntityTypeBuilder<EducationLevel> builder)
    {
        builder.ToTable("emp_education_level");
        builder.HasKey(x => x.Code);
        builder.Property(x => x.Code).HasMaxLength(32);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(128);
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(32);

        builder.HasData(
            new EducationLevel
            {
                Code = EmpCatalogSeed.ThptCode,
                Name = "Trung học phổ thông",
                Status = EducationLevelStatus.Active
            },
            new EducationLevel
            {
                Code = EmpCatalogSeed.CollegeCode,
                Name = "Cao đẳng",
                Status = EducationLevelStatus.Active
            },
            new EducationLevel
            {
                Code = EmpCatalogSeed.UniversityCode,
                Name = "Đại học",
                Status = EducationLevelStatus.Active
            },
            new EducationLevel
            {
                Code = EmpCatalogSeed.InactiveCode,
                Name = "Ngừng hiệu lực (test)",
                Status = EducationLevelStatus.Inactive
            });
    }
}
