using Common.Data.Configurations;
using EFCore.NamingConventions.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthAPI.Data.RoleClaim;


public class TRoleClaimConfiguration(INameRewriter nameRewriter) : EntityConfigurationBase<TRoleClaim>(nameRewriter)
{
    public override void Configure(EntityTypeBuilder<TRoleClaim> builder)
    {
        builder.ToTable(Rewrite(nameof(AuthDbContext.TRoleClaims)));

        builder.Property(rc => rc.ClaimType).HasMaxLength(256);
        builder.Property(rc => rc.ClaimValue).HasMaxLength(1024);
    }
}

