using Common.Data.Configurations;
using EFCore.NamingConventions.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthAPI.Data.Role;

public class TRoleConfiguration(INameRewriter nameRewriter) : EntityConfigurationBase<TRole>(nameRewriter)
{
    public override void Configure(EntityTypeBuilder<TRole> builder)
    {
        builder.ToTable(Rewrite(nameof(AuthDbContext.TRoles)));

        builder.Property(r => r.Name).HasMaxLength(256);
        builder.Property(r => r.NormalizedName).HasMaxLength(256);

        // Custom properties
        builder.Property(r => r.Description)
               .HasMaxLength(512);

        builder.Property(r => r.IsActive)
               .HasDefaultValue(true);

        builder.Property(r => r.CreatedOn);

        builder.Property(r => r.ModifiedOn);
    }
}

