using Hrm.Domain.Payroll;
using Hrm.Domain.Payroll.Entities;
using Hrm.Infrastructure.Persistence.Emp;
using Hrm.Infrastructure.Persistence.Pay;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hrm.Infrastructure.Persistence.Configurations;

internal sealed class PayAllowanceCatalogConfiguration : IEntityTypeConfiguration<PayAllowanceCatalog>
{
    public void Configure(EntityTypeBuilder<PayAllowanceCatalog> builder)
    {
        builder.ToTable("pay_allowance_catalog");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Code).IsRequired().HasMaxLength(64);
        builder.HasIndex(x => x.Code).IsUnique();
        builder.Property(x => x.Name).IsRequired().HasMaxLength(256);

        builder.HasData(
            new PayAllowanceCatalog
            {
                Id = PaySeed.CatalogMealId,
                Code = PayAllowanceCodes.Meal,
                Name = "Phụ cấp ăn trưa",
                IsActive = true
            },
            new PayAllowanceCatalog
            {
                Id = PaySeed.CatalogFuelId,
                Code = PayAllowanceCodes.Fuel,
                Name = "Phụ cấp xăng xe",
                IsActive = true
            });
    }
}

internal sealed class PayContractAllowanceConfiguration : IEntityTypeConfiguration<PayContractAllowance>
{
    public void Configure(EntityTypeBuilder<PayContractAllowance> builder)
    {
        builder.ToTable("pay_contract_allowance");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.EmployeeCode).IsRequired().HasMaxLength(64);
        builder.Property(x => x.Code).IsRequired().HasMaxLength(64);
        builder.Property(x => x.Amount).HasPrecision(18, 2);
        builder.HasIndex(x => new { x.EmployeeId, x.Code }).IsUnique();

        builder.HasData(
            new PayContractAllowance
            {
                Id = PaySeed.DevMealContractId,
                EmployeeId = EmpSeed.DevEmployeeId,
                EmployeeCode = EmpSeed.DevEmployeeCode,
                Code = PayAllowanceCodes.Meal,
                Amount = 730_000m
            });
    }
}

internal sealed class PayMonthlyAllowanceConfiguration : IEntityTypeConfiguration<PayMonthlyAllowance>
{
    public void Configure(EntityTypeBuilder<PayMonthlyAllowance> builder)
    {
        builder.ToTable("pay_monthly_allowance");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.PeriodYm).IsRequired().HasMaxLength(7);
        builder.Property(x => x.EmployeeCode).IsRequired().HasMaxLength(64);
        builder.Property(x => x.Code).IsRequired().HasMaxLength(64);
        builder.Property(x => x.Amount).HasPrecision(18, 2);
        builder.HasIndex(x => new { x.PeriodYm, x.EmployeeId, x.Code }).IsUnique();
    }
}
