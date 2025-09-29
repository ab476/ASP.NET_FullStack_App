using Common.Data.Configurations;
using EFCore.NamingConventions.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthAPI.Data.TableConfigurations;

public class TUserLoginConfiguration(INameRewriter nameRewriter) : EntityConfigurationBase<TUserLogin>(nameRewriter)
{
    public override void Configure(EntityTypeBuilder<TUserLogin> builder)
    {
        builder.ToTable(Rewrite(nameof(AuthDbContext.TUserLogins)));
    }
}

