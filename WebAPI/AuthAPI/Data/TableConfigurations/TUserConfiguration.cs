using EFCore.NamingConventions.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shared.Data.Configurations;

namespace AuthAPI.Data.TableConfigurations;

public class TUserConfiguration(INameRewriter nameRewriter) : EntityConfigurationBase<TUser>(nameRewriter)
{
    public override void Configure(EntityTypeBuilder<TUser> builder)
    {
        builder.ToTable(Rewrite(nameof(AuthDbContext.TUsers)));
    }
}

