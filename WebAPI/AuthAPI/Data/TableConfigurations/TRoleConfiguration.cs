using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthAPI.Data.TableConfigurations;

public class TRoleConfiguration : IEntityTypeConfiguration<TRole>
{
    public void Configure(EntityTypeBuilder<TRole> builder)
    {
        builder.ToTable("TRoles");
    }
}

