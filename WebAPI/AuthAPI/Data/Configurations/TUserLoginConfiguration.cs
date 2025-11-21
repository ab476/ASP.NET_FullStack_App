using AuthAPI.Data.Models;
using Common.Data.Configurations;
using Common.Features.NameHelper;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthAPI.Data.Configurations;

public class TUserLoginConfiguration(INameHelper nameRewriter) : EntityConfigurationBase<TUserLogin>(nameRewriter)
{
    public override void Configure(EntityTypeBuilder<TUserLogin> builder)
    {
        builder.ToTable(Rewrite(nameof(AuthDbContext.TUserLogins)));

        builder.Property(l => l.LoginProvider).HasMaxLength(128);
        builder.Property(l => l.ProviderKey).HasMaxLength(128);
        builder.Property(l => l.ProviderDisplayName).HasMaxLength(256);
    }
}