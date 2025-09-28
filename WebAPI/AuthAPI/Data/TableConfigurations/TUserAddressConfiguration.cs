using AuthAPI.Data.Tables;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthAPI.Data.TableConfigurations;

public class TUserAddressConfiguration : IEntityTypeConfiguration<TUserAddress>
{
    public void Configure(EntityTypeBuilder<TUserAddress> builder)
    {
        builder.HasKey(a => a.Id);

        builder.Property(a => a.Street).IsRequired();
        builder.Property(a => a.City).IsRequired();
        builder.Property(a => a.State).IsRequired();
        builder.Property(a => a.PostalCode).IsRequired();
        builder.Property(a => a.Country).IsRequired();

        builder.HasOne(a => a.User)
               .WithMany(u => u.Addresses)
               .HasForeignKey(a => a.UserId)
               .IsRequired();

        builder.Property(a => a.IsActive).HasDefaultValue(true);
    }
}

