using Common.Data.Configurations;
using Common.Features.NameHelper;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthAPI.Data.UserClaim;

public class TUserClaimConfiguration(INameHelper nameRewriter) : EntityConfigurationBase<TUserClaim>(nameRewriter)
{
    public override void Configure(EntityTypeBuilder<TUserClaim> builder)
    {
        builder.ToTable(Rewrite(nameof(AuthDbContext.TUserClaims)));

        builder.Property(uc => uc.ClaimType).HasMaxLength(256);
        builder.Property(uc => uc.ClaimValue).HasMaxLength(1024);
    }
}