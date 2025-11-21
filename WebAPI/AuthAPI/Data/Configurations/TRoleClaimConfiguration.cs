using AuthAPI.Data.Models;
using Common.Data.Configurations;
using Common.Features.NameHelper;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthAPI.Data.Configurations;


public class TRoleClaimConfiguration(INameHelper nameRewriter) : EntityConfigurationBase<TRoleClaim>(nameRewriter)
{
    public override void Configure(EntityTypeBuilder<TRoleClaim> builder)
    {
        builder.ToTable(Rewrite(nameof(AuthDbContext.TRoleClaims)));

        builder.Property(rc => rc.ClaimType).HasMaxLength(256);
        builder.Property(rc => rc.ClaimValue).HasMaxLength(1024);
    }
}