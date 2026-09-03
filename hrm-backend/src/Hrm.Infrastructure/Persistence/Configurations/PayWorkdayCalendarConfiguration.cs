using Hrm.Domain.Payroll.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hrm.Infrastructure.Persistence.Configurations;

internal sealed class PayWorkdayCalendarConfiguration : IEntityTypeConfiguration<PayWorkdayCalendar>
{
    public void Configure(EntityTypeBuilder<PayWorkdayCalendar> builder)
    {
        builder.ToTable("pay_workday_calendar");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.PeriodYm).IsRequired().HasMaxLength(7);
        builder.HasIndex(x => x.PeriodYm).IsUnique();
        builder.Property(x => x.StandardWorkDays).HasPrecision(5, 2);
    }
}
