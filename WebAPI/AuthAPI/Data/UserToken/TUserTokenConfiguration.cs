using AuthAPI.Data.UserToken;
using Common.Data.Configurations;
using Common.Features.NameHelper;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthAPI.Data.Tables;

public class TUserTokenConfiguration(INameHelper nameRewriter) : EntityConfigurationBase<TUserToken>(nameRewriter)
{
    public override void Configure(EntityTypeBuilder<TUserToken> builder)
    {
        builder.ToTable(Rewrite(nameof(AuthDbContext.TUserTokens)));

        builder.Property(t => t.LoginProvider)
              .IsRequired()
              .HasMaxLength(128);

        builder.Property(t => t.Name)
               .IsRequired()
               .HasMaxLength(128);

        builder.Property(t => t.Value)
               .HasMaxLength(512); // Adjust depending on token size

        builder.Property(t => t.UserId)
               .IsRequired();
    }
}