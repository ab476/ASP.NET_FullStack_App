using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthAPI.Data.TableConfigurations;

public class TUserTokenConfiguration : IEntityTypeConfiguration<TUserToken>
{
    public void Configure(EntityTypeBuilder<TUserToken> builder)
    {
        builder.ToTable("TUserTokens");
    }
}

