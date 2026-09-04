using Hrm.Domain.Lifecycle;
using Hrm.Domain.Lifecycle.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hrm.Infrastructure.Persistence.Configurations;

internal sealed class LifOffboardingCaseConfiguration : IEntityTypeConfiguration<LifOffboardingCase>
{
    public void Configure(EntityTypeBuilder<LifOffboardingCase> builder)
    {
        builder.ToTable("lif_offboarding_case");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.EmployeeCode).IsRequired().HasMaxLength(64);
        builder.Property(x => x.Source).IsRequired().HasMaxLength(64);
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(16);
        builder.Property(x => x.CreatedByIdpSubject).IsRequired().HasMaxLength(256);
        builder.Property(x => x.ConfirmedByIdpSubject).HasMaxLength(256);
        builder.Property(x => x.Note).HasMaxLength(2000);
        builder.HasIndex(x => x.EmployeeId);
        builder.HasIndex(x => x.Status);
    }
}
