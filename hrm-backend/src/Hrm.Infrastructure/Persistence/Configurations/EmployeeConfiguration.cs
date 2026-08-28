using Hrm.Domain.Employees;
using Hrm.Domain.Employees.Entities;
using Hrm.Infrastructure.Persistence.Emp;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hrm.Infrastructure.Persistence.Configurations;

internal sealed class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.ToTable("emp_employee");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.EmployeeCode).IsRequired().HasMaxLength(64);
        builder.HasIndex(x => x.EmployeeCode).IsUnique();
        builder.Property(x => x.FullName).HasMaxLength(256);
        builder.Property(x => x.Cccd).HasMaxLength(32);
        builder.HasIndex(x => x.Cccd).IsUnique();
        builder.Property(x => x.EmailCty).HasMaxLength(256);
        builder.HasIndex(x => x.EmailCty).IsUnique();
        builder.Property(x => x.TaxId).HasMaxLength(32);
        builder.HasIndex(x => x.TaxId).IsUnique();
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(32);

        builder.HasData(new Employee
        {
            Id = EmpSeed.DevEmployeeId,
            EmployeeCode = EmpSeed.DevEmployeeCode,
            FullName = "Dev IAM",
            EmailCty = "dev@company.local",
            Status = EmployeeStatus.Active
        });
    }
}
