using Common.Data.Configurations;
using EFCore.NamingConventions.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthAPI.Data.TableConfigurations;


public class TRoleClaimConfiguration(INameRewriter nameRewriter) : EntityConfigurationBase<TRoleClaim>(nameRewriter)
{
    public override void Configure(EntityTypeBuilder<TRoleClaim> builder)
    {
        builder.ToTable(Rewrite(nameof(AuthDbContext.TRoleClaims)));
    }
}

