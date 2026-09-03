using Hrm.Domain.Probation.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hrm.Infrastructure.Persistence.Configurations;

internal sealed class ProbationReminderConfiguration : IEntityTypeConfiguration<ProbationReminder>
{
    public void Configure(EntityTypeBuilder<ProbationReminder> builder)
    {
        builder.ToTable("prb_reminder");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Kind).HasConversion<string>().HasMaxLength(8);
        builder.Property(x => x.EmployeeCode).IsRequired().HasMaxLength(64);
        builder.Property(x => x.AssigneeEmployeeCode).HasMaxLength(64);
        builder.Property(x => x.InAppMessage).IsRequired().HasMaxLength(1024);
        builder.Property(x => x.EmailTo).IsRequired().HasMaxLength(512);
        builder.Property(x => x.Channel).IsRequired().HasMaxLength(64);
        builder.Property(x => x.CreatedByIdpSubject).IsRequired().HasMaxLength(256);
        builder.HasIndex(x => new { x.EmployeeId, x.Kind, x.ProbationEndDate }).IsUnique();
        builder.HasIndex(x => x.AsOfDate);
    }
}
