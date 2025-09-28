using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthAPI.Data.TableConfigurations;

public class TUserLoginConfiguration : IEntityTypeConfiguration<TUserLogin>
{
    public void Configure(EntityTypeBuilder<TUserLogin> builder)
    {
        builder.ToTable("TUserLogins");
    }
}

