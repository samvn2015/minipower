using Hrm.Domain.Employees.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hrm.Infrastructure.Persistence.Configurations;

internal sealed class EmpAuditLogConfiguration : IEntityTypeConfiguration<EmpAuditLog>
{
    public void Configure(EntityTypeBuilder<EmpAuditLog> builder)
    {
        builder.ToTable("emp_audit_log");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Action).IsRequired().HasMaxLength(64);
        builder.Property(x => x.ActorIdpSubject).IsRequired().HasMaxLength(128);
        builder.Property(x => x.Detail).HasMaxLength(1024);
        builder.HasIndex(x => x.EmployeeId);
        builder.HasIndex(x => x.OccurredAtUtc);
    }
}
