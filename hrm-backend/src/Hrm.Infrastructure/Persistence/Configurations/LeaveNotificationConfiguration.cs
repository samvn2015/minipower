using Hrm.Domain.Leave.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hrm.Infrastructure.Persistence.Configurations;

internal sealed class LeaveNotificationConfiguration : IEntityTypeConfiguration<LeaveNotification>
{
    public void Configure(EntityTypeBuilder<LeaveNotification> builder)
    {
        builder.ToTable("lev_notification_outbox");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.EventType).IsRequired().HasMaxLength(64);
        builder.Property(x => x.Channel).IsRequired().HasMaxLength(32);
        builder.Property(x => x.Message).IsRequired().HasMaxLength(2000);
        builder.HasIndex(x => x.EmployeeId);
        builder.HasIndex(x => x.LeaveRequestId);
    }
}
