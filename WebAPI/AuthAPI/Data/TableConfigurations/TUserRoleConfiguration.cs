using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthAPI.Data.TableConfigurations;

public class TUserRoleConfiguration : IEntityTypeConfiguration<TUserRole>
{
    public void Configure(EntityTypeBuilder<TUserRole> builder)
    {
        builder.ToTable("TUserRoles");
    }
}

