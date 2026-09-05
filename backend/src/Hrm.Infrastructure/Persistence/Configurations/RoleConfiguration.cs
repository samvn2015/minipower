using Hrm.Domain.Identity.Entities;
using Hrm.Infrastructure.Persistence.Iam;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hrm.Infrastructure.Persistence.Configurations;

internal sealed class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("iam_role");
        builder.HasKey(x => x.RoleCode);
        builder.Property(x => x.RoleCode).HasMaxLength(64);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(128);

        builder.HasData(
            new Role { RoleCode = IamSeed.Roles.Nv, Name = "Nhân viên" },
            new Role { RoleCode = IamSeed.Roles.Lm, Name = "Line Manager" },
            new Role { RoleCode = IamSeed.Roles.Hr, Name = "HR / C&B" },
            new Role { RoleCode = IamSeed.Roles.It, Name = "IT Admin" },
            new Role { RoleCode = IamSeed.Roles.Pgd, Name = "PGD / BGĐ" });
    }
}
