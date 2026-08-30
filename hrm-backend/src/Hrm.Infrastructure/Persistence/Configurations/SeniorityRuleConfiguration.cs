using Hrm.Domain.Employees;
using Hrm.Domain.Employees.Entities;
using Hrm.Infrastructure.Persistence.Emp;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hrm.Infrastructure.Persistence.Configurations;

internal sealed class SeniorityRuleConfiguration : IEntityTypeConfiguration<SeniorityRule>
{
    public void Configure(EntityTypeBuilder<SeniorityRule> builder)
    {
        builder.ToTable("emp_seniority_rule");
        builder.HasKey(x => x.Code);
        builder.Property(x => x.Code).HasMaxLength(32);
        builder.Property(x => x.BasisType).HasConversion<string>().HasMaxLength(32);
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(32);

        builder.HasData(new SeniorityRule
        {
            Code = EmpCatalogSeed.DefaultSeniorityRule,
            BasisType = SeniorityBasisType.ContractStartDate,
            Status = SeniorityRuleStatus.Active
        });
    }
}
