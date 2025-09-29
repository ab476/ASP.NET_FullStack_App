using Common.Data.Configurations;
using EFCore.NamingConventions.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthAPI.Data.TableConfigurations;

public class TUserClaimConfiguration(INameRewriter nameRewriter) : EntityConfigurationBase<TUserClaim>(nameRewriter)
{
    public override void Configure(EntityTypeBuilder<TUserClaim> builder)
    {
        builder.ToTable(Rewrite(nameof(AuthDbContext.TUserClaims)));
    }
}

