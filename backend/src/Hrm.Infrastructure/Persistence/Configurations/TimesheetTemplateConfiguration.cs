using Hrm.Domain.Timekeeping;
using Hrm.Domain.Timekeeping.Entities;
using Hrm.Infrastructure.Persistence.Tim;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hrm.Infrastructure.Persistence.Configurations;

internal sealed class TimesheetTemplateVersionConfiguration : IEntityTypeConfiguration<TimesheetTemplateVersion>
{
    public void Configure(EntityTypeBuilder<TimesheetTemplateVersion> builder)
    {
        builder.ToTable("tim_template_version");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.VersionCode).IsRequired().HasMaxLength(64);
        builder.HasIndex(x => x.VersionCode).IsUnique();
        builder.Property(x => x.Name).IsRequired().HasMaxLength(256);
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(32);
        builder.Property(x => x.PublishedByIdpSubject).HasMaxLength(256);
        builder.HasIndex(x => x.Status);

        builder.HasData(new TimesheetTemplateVersion
        {
            Id = TimSeed.TemplateV1Id,
            VersionCode = TimSeed.TemplateV1Code,
            Name = "Mẫu công V1 (seed)",
            Status = TimesheetTemplateStatus.Active,
            PublishedAtUtc = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            PublishedByIdpSubject = "seed"
        });
    }
}

internal sealed class TimesheetTemplateColumnConfiguration : IEntityTypeConfiguration<TimesheetTemplateColumn>
{
    public void Configure(EntityTypeBuilder<TimesheetTemplateColumn> builder)
    {
        builder.ToTable("tim_template_column");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.ColumnKey).IsRequired().HasMaxLength(64);
        builder.Property(x => x.DisplayName).IsRequired().HasMaxLength(256);
        builder.Property(x => x.MapsTo).IsRequired().HasMaxLength(64);
        builder.HasIndex(x => new { x.TemplateVersionId, x.ColumnKey }).IsUnique();
        builder.HasOne(x => x.TemplateVersion)
            .WithMany(x => x.Columns)
            .HasForeignKey(x => x.TemplateVersionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasData(
            new TimesheetTemplateColumn
            {
                Id = TimSeed.ColEmployeeCodeId,
                TemplateVersionId = TimSeed.TemplateV1Id,
                ColumnKey = "mnv",
                DisplayName = "Mã NV",
                SortOrder = 1,
                IsRequired = true,
                MapsTo = "EmployeeCode"
            },
            new TimesheetTemplateColumn
            {
                Id = TimSeed.ColWorkDaysId,
                TemplateVersionId = TimSeed.TemplateV1Id,
                ColumnKey = "n_thuc",
                DisplayName = "Ngày công thực",
                SortOrder = 2,
                IsRequired = true,
                MapsTo = "WorkDays"
            },
            new TimesheetTemplateColumn
            {
                Id = TimSeed.ColOt15Id,
                TemplateVersionId = TimSeed.TemplateV1Id,
                ColumnKey = "ot_15",
                DisplayName = "OT 1.5",
                SortOrder = 3,
                IsRequired = false,
                MapsTo = "Ot15"
            },
            new TimesheetTemplateColumn
            {
                Id = TimSeed.ColOt20Id,
                TemplateVersionId = TimSeed.TemplateV1Id,
                ColumnKey = "ot_20",
                DisplayName = "OT 2.0",
                SortOrder = 4,
                IsRequired = false,
                MapsTo = "Ot20"
            },
            new TimesheetTemplateColumn
            {
                Id = TimSeed.ColOt30Id,
                TemplateVersionId = TimSeed.TemplateV1Id,
                ColumnKey = "ot_30",
                DisplayName = "OT 3.0",
                SortOrder = 5,
                IsRequired = false,
                MapsTo = "Ot30"
            },
            new TimesheetTemplateColumn
            {
                Id = TimSeed.ColOtUnclassifiedId,
                TemplateVersionId = TimSeed.TemplateV1Id,
                ColumnKey = "ot_unclassified",
                DisplayName = "OT chưa phân loại",
                SortOrder = 6,
                IsRequired = false,
                MapsTo = "OtUnclassified"
            });
    }
}
