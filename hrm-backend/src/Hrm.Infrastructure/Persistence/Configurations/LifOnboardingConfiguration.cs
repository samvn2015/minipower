using Hrm.Domain.Lifecycle.Entities;
using Hrm.Infrastructure.Persistence.Lifecycle;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hrm.Infrastructure.Persistence.Configurations;

internal sealed class LifOnboardingCaseConfiguration : IEntityTypeConfiguration<LifOnboardingCase>
{
    public void Configure(EntityTypeBuilder<LifOnboardingCase> builder)
    {
        builder.ToTable("lif_onboarding_case");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.EmployeeCode).IsRequired().HasMaxLength(64);
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(16);
        builder.Property(x => x.CreatedByIdpSubject).IsRequired().HasMaxLength(256);
        builder.Property(x => x.ClosedByIdpSubject).HasMaxLength(256);
        builder.Property(x => x.Note).HasMaxLength(2000);
        builder.HasIndex(x => x.EmployeeId);
        builder.HasIndex(x => x.Status);
    }
}

internal sealed class LifOnChecklistConfiguration :
    IEntityTypeConfiguration<LifOnChecklistItem>,
    IEntityTypeConfiguration<LifOnChecklistTick>
{
    public void Configure(EntityTypeBuilder<LifOnChecklistItem> builder)
    {
        builder.ToTable("lif_on_checklist_item");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Code).IsRequired().HasMaxLength(64);
        builder.HasIndex(x => x.Code).IsUnique();
        builder.Property(x => x.Name).IsRequired().HasMaxLength(256);
        builder.HasData(LifSeed.OnChecklistItems());
    }

    public void Configure(EntityTypeBuilder<LifOnChecklistTick> builder)
    {
        builder.ToTable("lif_on_checklist_tick");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.ItemCode).IsRequired().HasMaxLength(64);
        builder.Property(x => x.CheckedByIdpSubject).HasMaxLength(256);
        builder.HasIndex(x => new { x.OnboardingCaseId, x.ItemCode }).IsUnique();
    }
}
