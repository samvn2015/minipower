using Hrm.Domain.Identity.Entities;
using Hrm.Infrastructure.Persistence.Iam;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hrm.Infrastructure.Persistence.Configurations;

internal sealed class AccountRoleConfiguration : IEntityTypeConfiguration<AccountRole>
{
    public void Configure(EntityTypeBuilder<AccountRole> builder)
    {
        builder.ToTable("iam_account_role");
        builder.HasKey(x => new { x.AccountId, x.RoleCode });
        builder.Property(x => x.RoleCode).HasMaxLength(64);

        builder.HasOne(x => x.Account)
            .WithMany(x => x.AccountRoles)
            .HasForeignKey(x => x.AccountId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Role)
            .WithMany(x => x.AccountRoles)
            .HasForeignKey(x => x.RoleCode)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasData(
            new AccountRole { AccountId = IamSeed.DevAccountId, RoleCode = IamSeed.Roles.Nv },
            new AccountRole { AccountId = IamSeed.DevAccountId, RoleCode = IamSeed.Roles.Hr },
            new AccountRole { AccountId = IamSeed.ItDevAccountId, RoleCode = IamSeed.Roles.It },
            new AccountRole { AccountId = IamSeed.LmDevAccountId, RoleCode = IamSeed.Roles.Nv },
            new AccountRole { AccountId = IamSeed.LmDevAccountId, RoleCode = IamSeed.Roles.Lm });
    }
}
