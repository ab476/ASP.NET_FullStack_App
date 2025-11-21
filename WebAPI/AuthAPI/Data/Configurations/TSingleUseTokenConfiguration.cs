using AuthAPI.Data.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthAPI.Data.Configurations;

public class TSingleUseTokenConfiguration : IEntityTypeConfiguration<TSingleUseToken>
{
    public void Configure(EntityTypeBuilder<TSingleUseToken> builder)
    {

        builder.HasKey(x => x.Id);

        builder.Property(x => x.TokenHash)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(x => x.Purpose)
            .HasMaxLength(100)
            .IsRequired();

        builder.HasIndex(x => x.TokenHash);

        builder.HasOne(x => x.User)
            .WithMany(u => u.SingleUseTokens)
            .HasForeignKey(x => x.UserId);
    }
}

