using Hrm.Domain.Leave;
using Hrm.Domain.Leave.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hrm.Infrastructure.Persistence.Configurations;

internal sealed class LeaveRequestConfiguration : IEntityTypeConfiguration<LeaveRequest>
{
    public void Configure(EntityTypeBuilder<LeaveRequest> builder)
    {
        builder.ToTable("lev_leave_request");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.LeaveTypeCode).IsRequired().HasMaxLength(32);
        builder.Property(x => x.DayPart).HasConversion<string>().HasMaxLength(16);
        builder.Property(x => x.TotalDays).HasPrecision(5, 1);
        builder.Property(x => x.Reason).IsRequired().HasMaxLength(2000);
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(32);
        builder.Property(x => x.SubmittedAtUtc).IsRequired();
        builder.Property(x => x.C1ReviewedByIdpSubject).HasMaxLength(256);
        builder.Property(x => x.C1ReviewNote).HasMaxLength(2000);
        builder.Property(x => x.C2ReviewedByIdpSubject).HasMaxLength(256);
        builder.Property(x => x.C2ReviewNote).HasMaxLength(2000);
        builder.Property(x => x.AttachmentFileName).HasMaxLength(512);
        builder.HasIndex(x => x.EmployeeId);
        builder.HasOne(x => x.LeaveType)
            .WithMany()
            .HasForeignKey(x => x.LeaveTypeCode)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
