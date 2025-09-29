using Common.Data.Configurations;
using EFCore.NamingConventions.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthAPI.Data.TableConfigurations;

public class TRoleConfiguration(INameRewriter nameRewriter) : EntityConfigurationBase<TRole>(nameRewriter)
{
    public override void Configure(EntityTypeBuilder<TRole> builder)
    {
        builder.ToTable(Rewrite(nameof(AuthDbContext.TRoles)));
    }
}

