using EFCore.NamingConventions.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shared.Data.Configurations;

namespace AuthAPI.Data.TableConfigurations;

public class TUserRoleConfiguration(INameRewriter nameRewriter) : EntityConfigurationBase<TUserRole>(nameRewriter)
{
    public override void Configure(EntityTypeBuilder<TUserRole> builder)
    {
        builder.ToTable(Rewrite(nameof(AuthDbContext.TUserRoles)));
    }
}

