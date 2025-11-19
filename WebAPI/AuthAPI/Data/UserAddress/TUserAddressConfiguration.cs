using Common.Data.Configurations;
using Common.Features.NameHelper;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthAPI.Data.UserAddress;

public class TUserAddressConfiguration(INameHelper nameRewriter) : EntityConfigurationBase<TUserAddress>(nameRewriter)
{
    public override void Configure(EntityTypeBuilder<TUserAddress> builder)
    {
        builder.HasKey(a => a.Id);

        builder.Property(a => a.Street)
              .IsRequired()
              .HasMaxLength(250);

        builder.Property(a => a.City)
               .IsRequired()
               .HasMaxLength(100);

        builder.Property(a => a.State)
               .IsRequired()
               .HasMaxLength(100);

        builder.Property(a => a.PostalCode)
               .IsRequired()
               .HasMaxLength(20);

        builder.Property(a => a.Country)
               .IsRequired()
               .HasMaxLength(100);

        builder.Property(a => a.CreatedOn);

        builder.Property(a => a.ModifiedOn);

        builder.HasOne(a => a.User)
               .WithMany(u => u.Addresses)
               .HasForeignKey(a => a.UserId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade); // Optional: enforce cascade delete
    }
}