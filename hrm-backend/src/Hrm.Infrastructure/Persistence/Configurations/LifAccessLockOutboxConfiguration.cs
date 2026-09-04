using Hrm.Domain.Lifecycle.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hrm.Infrastructure.Persistence.Configurations;

internal sealed class LifAccessLockOutboxConfiguration : IEntityTypeConfiguration<LifAccessLockOutbox>
{
    public void Configure(EntityTypeBuilder<LifAccessLockOutbox> builder)
    {
        builder.ToTable("lif_access_lock_outbox");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.EmployeeCode).IsRequired().HasMaxLength(64);
        builder.Property(x => x.TargetSystems).IsRequired().HasMaxLength(64);
        builder.Property(x => x.Channel).IsRequired().HasMaxLength(32);
        builder.Property(x => x.CrReason).HasMaxLength(2000);
        builder.Property(x => x.CreatedByIdpSubject).IsRequired().HasMaxLength(256);
        builder.HasIndex(x => x.CaseId);
        builder.HasIndex(x => x.AsOfDate);
    }
}
