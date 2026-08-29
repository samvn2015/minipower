using Hrm.Domain.Employees;
using Hrm.Domain.Employees.Entities;
using Hrm.Infrastructure.Persistence.Emp;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hrm.Infrastructure.Persistence.Configurations;

internal sealed class OrgUnitConfiguration : IEntityTypeConfiguration<OrgUnit>
{
    public void Configure(EntityTypeBuilder<OrgUnit> builder)
    {
        builder.ToTable("emp_org_unit");
        builder.HasKey(x => x.Code);
        builder.Property(x => x.Code).HasMaxLength(32);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(256);
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(32);

        builder.HasData(
            new OrgUnit
            {
                Code = EmpOrgSeed.HqCode,
                Name = "Trụ sở HN",
                Status = OrgUnitStatus.Active
            },
            new OrgUnit
            {
                Code = EmpOrgSeed.InactiveCode,
                Name = "Đơn vị ngừng",
                Status = OrgUnitStatus.Inactive
            });
    }
}
