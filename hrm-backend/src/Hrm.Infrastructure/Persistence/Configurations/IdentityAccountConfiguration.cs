using Hrm.Domain.Identity;
using Hrm.Domain.Identity.Entities;
using Hrm.Infrastructure.Persistence.Emp;
using Hrm.Infrastructure.Persistence.Iam;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hrm.Infrastructure.Persistence.Configurations;

internal sealed class IdentityAccountConfiguration : IEntityTypeConfiguration<IdentityAccount>
{
    public void Configure(EntityTypeBuilder<IdentityAccount> builder)
    {
        builder.ToTable("iam_identity_account");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.IdpSubject).IsRequired().HasMaxLength(256);
        builder.HasIndex(x => x.IdpSubject).IsUnique();
        builder.Property(x => x.EmailCty).HasMaxLength(256);
        builder.Property(x => x.DisplayName).HasMaxLength(256);
        builder.Property(x => x.EmployeeCode).HasMaxLength(64);
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(32);

        builder.HasData(
            new IdentityAccount
            {
                Id = IamSeed.DevAccountId,
                IdpSubject = IamSeed.DevIdpSubject,
                EmailCty = "dev@company.local",
                DisplayName = "Dev IAM",
                EmployeeCode = "MNV-DEV",
                Status = IdentityAccountStatus.Active
            },
            new IdentityAccount
            {
                Id = IamSeed.ItDevAccountId,
                IdpSubject = IamSeed.ItDevIdpSubject,
                EmailCty = "it@company.local",
                DisplayName = "IT Dev",
                EmployeeCode = null,
                Status = IdentityAccountStatus.Active
            },
            new IdentityAccount
            {
                Id = IamSeed.LmDevAccountId,
                IdpSubject = IamSeed.LmDevIdpSubject,
                EmailCty = "handover@company.local",
                DisplayName = "Handover NV (LM)",
                EmployeeCode = EmpSeed.HandoverEmployeeCode,
                Status = IdentityAccountStatus.Active
            });
    }
}
