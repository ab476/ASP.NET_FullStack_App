using AuthAPI.Data.Models;
using Common.Data.Configurations;
using Common.Features.NameHelper;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthAPI.Data.Configurations;

public class TUserConfiguration(INameHelper nameRewriter) : EntityConfigurationBase<TUser>(nameRewriter)
{
    public override void Configure(EntityTypeBuilder<TUser> builder)
    {
        builder.ToTable(Rewrite(nameof(AuthDbContext.TUsers)));

        builder.Property(u => u.UserName).HasMaxLength(256);
        builder.Property(u => u.NormalizedUserName).HasMaxLength(256);
        builder.Property(u => u.Email).HasMaxLength(256);
        builder.Property(u => u.NormalizedEmail).HasMaxLength(256);
    }
}