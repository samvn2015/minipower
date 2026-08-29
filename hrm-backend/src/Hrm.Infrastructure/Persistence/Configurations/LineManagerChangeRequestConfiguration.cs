using Hrm.Domain.Employees;
using Hrm.Domain.Employees.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hrm.Infrastructure.Persistence.Configurations;

internal sealed class LineManagerChangeRequestConfiguration : IEntityTypeConfiguration<LineManagerChangeRequest>
{
    public void Configure(EntityTypeBuilder<LineManagerChangeRequest> builder)
    {
        builder.ToTable("emp_line_manager_change");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(32);
        builder.Property(x => x.RequestedByIdpSubject).IsRequired().HasMaxLength(256);
        builder.Property(x => x.ReviewedByIdpSubject).HasMaxLength(256);
        builder.Property(x => x.ReviewNote).HasMaxLength(512);
        builder.HasIndex(x => new { x.EmployeeId, x.Status });

        builder.HasOne(x => x.Employee)
            .WithMany(x => x.LineManagerChangeRequests)
            .HasForeignKey(x => x.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
