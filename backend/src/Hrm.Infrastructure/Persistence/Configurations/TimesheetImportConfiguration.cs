using Hrm.Domain.Timekeeping;
using Hrm.Domain.Timekeeping.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hrm.Infrastructure.Persistence.Configurations;

internal sealed class TimesheetImportBatchConfiguration : IEntityTypeConfiguration<TimesheetImportBatch>
{
    public void Configure(EntityTypeBuilder<TimesheetImportBatch> builder)
    {
        builder.ToTable("tim_import_batch");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.PeriodYm).IsRequired().HasMaxLength(7);
        builder.Property(x => x.TemplateVersionCode).IsRequired().HasMaxLength(64);
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(32);
        builder.Property(x => x.UploadedByIdpSubject).IsRequired().HasMaxLength(256);
        builder.Property(x => x.FileName).HasMaxLength(512);
        builder.HasIndex(x => x.PeriodYm);
    }
}

internal sealed class TimesheetImportRowConfiguration : IEntityTypeConfiguration<TimesheetImportRow>
{
    public void Configure(EntityTypeBuilder<TimesheetImportRow> builder)
    {
        builder.ToTable("tim_import_row");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.EmployeeCode).HasMaxLength(64);
        builder.Property(x => x.WorkDays).HasPrecision(5, 2);
        builder.Property(x => x.Ot15).HasPrecision(5, 2);
        builder.Property(x => x.Ot20).HasPrecision(5, 2);
        builder.Property(x => x.Ot30).HasPrecision(5, 2);
        builder.Property(x => x.OtUnclassified).HasPrecision(5, 2);
        builder.Property(x => x.ErrorCode).HasMaxLength(64);
        builder.Property(x => x.ErrorMessage).HasMaxLength(1000);
        builder.HasOne(x => x.Batch)
            .WithMany(x => x.Rows)
            .HasForeignKey(x => x.BatchId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasIndex(x => new { x.BatchId, x.RowNumber }).IsUnique();
    }
}

internal sealed class TimesheetPeriodConfiguration : IEntityTypeConfiguration<TimesheetPeriod>
{
    public void Configure(EntityTypeBuilder<TimesheetPeriod> builder)
    {
        builder.ToTable("tim_period");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.PeriodYm).IsRequired().HasMaxLength(7);
        builder.HasIndex(x => x.PeriodYm).IsUnique();
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(32);
        builder.Property(x => x.CommittedByIdpSubject).HasMaxLength(256);
        builder.Property(x => x.ClosedByIdpSubject).HasMaxLength(256);
    }
}

internal sealed class TimesheetLineConfiguration : IEntityTypeConfiguration<TimesheetLine>
{
    public void Configure(EntityTypeBuilder<TimesheetLine> builder)
    {
        builder.ToTable("tim_timesheet_line");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.EmployeeCode).IsRequired().HasMaxLength(64);
        builder.Property(x => x.WorkDays).HasPrecision(5, 2);
        builder.Property(x => x.Ot15).HasPrecision(5, 2);
        builder.Property(x => x.Ot20).HasPrecision(5, 2);
        builder.Property(x => x.Ot30).HasPrecision(5, 2);
        builder.Property(x => x.OtUnclassified).HasPrecision(5, 2);
        builder.Property(x => x.LeaveDaysPaid).HasPrecision(5, 2);
        builder.Property(x => x.LeaveDaysUnpaid).HasPrecision(5, 2);
        builder.Property(x => x.LeaveDaysOther).HasPrecision(5, 2);
        builder.HasOne(x => x.Period)
            .WithMany(x => x.Lines)
            .HasForeignKey(x => x.PeriodId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasIndex(x => new { x.PeriodId, x.EmployeeId }).IsUnique();
    }
}
