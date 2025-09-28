using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthAPI.Data.TableConfigurations;

public class TUserClaimConfiguration : IEntityTypeConfiguration<TUserClaim>
{
    public void Configure(EntityTypeBuilder<TUserClaim> builder)
    {
        builder.ToTable("TUserClaims");
    }
}

