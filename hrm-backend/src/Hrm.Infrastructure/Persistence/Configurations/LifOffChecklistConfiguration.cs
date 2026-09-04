using Hrm.Domain.Lifecycle.Entities;
using Hrm.Infrastructure.Persistence.Lifecycle;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hrm.Infrastructure.Persistence.Configurations;

internal sealed class LifOffChecklistItemConfiguration : IEntityTypeConfiguration<LifOffChecklistItem>
{
    public void Configure(EntityTypeBuilder<LifOffChecklistItem> builder)
    {
        builder.ToTable("lif_off_checklist_item");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Code).IsRequired().HasMaxLength(64);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(256);
        builder.HasIndex(x => x.Code).IsUnique();
        builder.HasData(LifSeed.OffChecklistItems());
    }
}

internal sealed class LifOffChecklistTickConfiguration : IEntityTypeConfiguration<LifOffChecklistTick>
{
    public void Configure(EntityTypeBuilder<LifOffChecklistTick> builder)
    {
        builder.ToTable("lif_off_checklist_tick");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.ItemCode).IsRequired().HasMaxLength(64);
        builder.Property(x => x.CheckedByIdpSubject).HasMaxLength(256);
        builder.HasIndex(x => new { x.OffboardingCaseId, x.ItemCode }).IsUnique();
    }
}
