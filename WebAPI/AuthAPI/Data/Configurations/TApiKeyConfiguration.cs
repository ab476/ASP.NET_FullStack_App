using AuthAPI.Data.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthAPI.Data.Configurations;

public class TApiKeyConfiguration : IEntityTypeConfiguration<TApiKey>
{
    public void Configure(EntityTypeBuilder<TApiKey> builder)
    {

        builder.HasKey(x => x.Id);

        builder.Property(x => x.KeyHash)
            .HasMaxLength(256)
            .IsRequired();

        builder.HasIndex(x => x.KeyHash);

        builder.HasOne(x => x.User)
            .WithMany(u => u.ApiKeys)
            .HasForeignKey(x => x.UserId);
    }
}

