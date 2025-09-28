using AuthAPI.Data.Tables;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.ComponentModel;

namespace AuthAPI.Data;


public class AuthDbContext
    : IdentityDbContext<
        TUser,
        TRole,
        Guid,
        TUserClaim,
        TUserRole,
        TUserLogin,
        TRoleClaim,
        TUserToken>
{
    public DbSet<TUserAddress> TUserAddresses { get; set; }
    public AuthDbContext(DbContextOptions<AuthDbContext> options)
        : base(options) { }
    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);

        configurationBuilder.Properties<Guid>().HaveConversion<GuidToBytesConverter>();
            
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        {

        }
        // Apply all IEntityTypeConfiguration<> implementations automatically
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AuthDbContext).Assembly);
    }
}

