using AuthAPI.Data.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthAPI.Data.Configurations;

public class TRefreshTokenConfiguration : IEntityTypeConfiguration<TRefreshToken>
{
    public void Configure(EntityTypeBuilder<TRefreshToken> builder)
    {

        builder.HasKey(x => x.Id);

        builder.Property(x => x.TokenHash)
            .HasMaxLength(256)
            .IsRequired();

        builder.HasIndex(x => x.TokenHash);
        builder.HasIndex(x => x.UserId);

        builder.HasOne(x => x.User)
            .WithMany(u => u.RefreshTokens)
            .HasForeignKey(x => x.UserId);
    }
}

