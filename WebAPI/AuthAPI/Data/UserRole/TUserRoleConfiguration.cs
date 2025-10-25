using Common.Data.Configurations;
using EFCore.NamingConventions.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthAPI.Data.UserRole;

public class TUserRoleConfiguration(INameRewriter nameRewriter) : EntityConfigurationBase<TUserRole>(nameRewriter)
{
    public override void Configure(EntityTypeBuilder<TUserRole> builder)
    {
        builder.ToTable(Rewrite(nameof(AuthDbContext.TUserRoles)));

        // Foreign keys
        builder.Property(ur => ur.UserId).IsRequired();
        builder.Property(ur => ur.RoleId).IsRequired();

        // Optional indexes for fast lookups
        builder.HasIndex(ur => ur.RoleId).HasDatabaseName(Rewrite("IX_UserRoles_RoleId"));
    }
}

