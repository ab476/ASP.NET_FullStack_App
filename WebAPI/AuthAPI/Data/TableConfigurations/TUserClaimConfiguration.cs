using EFCore.NamingConventions.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shared.Data.Configurations;

namespace AuthAPI.Data.TableConfigurations;

public class TUserClaimConfiguration(INameRewriter nameRewriter) : EntityConfigurationBase<TUserClaim>(nameRewriter)
{
    public override void Configure(EntityTypeBuilder<TUserClaim> builder)
    {
        builder.ToTable(Rewrite(nameof(AuthDbContext.TUserClaims)));
    }
}

