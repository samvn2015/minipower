using Hrm.Domain.Payroll.Entities;
using Hrm.Domain.Payroll.Repositories;
using Hrm.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hrm.Infrastructure.Persistence.Configurations;

internal sealed class PayExportOutboxConfiguration : IEntityTypeConfiguration<PayExportOutbox>
{
    public void Configure(EntityTypeBuilder<PayExportOutbox> builder)
    {
        builder.ToTable("pay_export_outbox");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.PeriodYm).IsRequired().HasMaxLength(7);
        builder.Property(x => x.EmployeeCode).IsRequired().HasMaxLength(64);
        builder.Property(x => x.ToAddress).IsRequired().HasMaxLength(256);
        builder.Property(x => x.CcAddress).HasMaxLength(512);
        builder.Property(x => x.Channel).IsRequired().HasMaxLength(32);
        builder.Property(x => x.Subject).IsRequired().HasMaxLength(256);
        builder.Property(x => x.PdfFileName).HasMaxLength(256);
        builder.Property(x => x.CreatedByIdpSubject).IsRequired().HasMaxLength(256);
        builder.HasIndex(x => x.PeriodYm);
    }
}
