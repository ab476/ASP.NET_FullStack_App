using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthAPI.Data.TableConfigurations;

public class TUserConfiguration : IEntityTypeConfiguration<TUser>
{
    public void Configure(EntityTypeBuilder<TUser> builder)
    {
        builder.ToTable("TUsers");
    }
}

