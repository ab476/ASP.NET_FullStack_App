using Common.Data.Configurations;
using EFCore.NamingConventions.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthAPI.Data.TableConfigurations;

public class TUserTokenConfiguration(INameRewriter nameRewriter) : EntityConfigurationBase<TUserToken>(nameRewriter)
{
    public override void Configure(EntityTypeBuilder<TUserToken> builder)
    {
        builder.ToTable(Rewrite(nameof(AuthDbContext.TUserTokens)));
    }
}

