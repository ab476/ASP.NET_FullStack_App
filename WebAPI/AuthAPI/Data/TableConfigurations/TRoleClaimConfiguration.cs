using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthAPI.Data.TableConfigurations;


public class TRoleClaimConfiguration : IEntityTypeConfiguration<TRoleClaim>
{
    public void Configure(EntityTypeBuilder<TRoleClaim> builder)
    {
        builder.ToTable(nameof(TRoleClaim));
    }
}

