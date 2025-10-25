using Common.Data.Configurations;
using EFCore.NamingConventions.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthAPI.Data.UserClaim;

public class TUserClaimConfiguration(INameRewriter nameRewriter) : EntityConfigurationBase<TUserClaim>(nameRewriter)
{
    public override void Configure(EntityTypeBuilder<TUserClaim> builder)
    {
        builder.ToTable(Rewrite(nameof(AuthDbContext.TUserClaims)));

        builder.Property(uc => uc.ClaimType).HasMaxLength(256);
        builder.Property(uc => uc.ClaimValue).HasMaxLength(1024);
    }
}

