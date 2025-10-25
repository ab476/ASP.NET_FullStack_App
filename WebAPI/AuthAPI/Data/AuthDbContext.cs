using AuthAPI.Data.Role;
using AuthAPI.Data.RoleClaim;
using AuthAPI.Data.User;
using AuthAPI.Data.UserAddress;
using AuthAPI.Data.UserClaim;
using AuthAPI.Data.UserLogin;
using AuthAPI.Data.UserRole;
using EFCore.NamingConventions.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace AuthAPI.Data;


public class AuthDbContext(DbContextOptions<AuthDbContext> options, INameRewriter nameRewriter, IEntityConfigurationAggregator aggregator)
        : IdentityDbContext<
        TUser,
        TRole,
        Guid,
        TUserClaim,
        TUserRole,
        TUserLogin,
        TRoleClaim,
        TUserToken>(options)
{
    public DbSet<TUser> TUsers => base.Users;
    public DbSet<TRole> TRoles => base.Roles;
    public DbSet<TUserClaim> TUserClaims => base.UserClaims;
    public DbSet<TUserRole> TUserRoles => base.UserRoles;
    public DbSet<TUserLogin> TUserLogins => base.UserLogins;
    public DbSet<TRoleClaim> TRoleClaims => base.RoleClaims;
    public DbSet<TUserToken> TUserTokens => base.UserTokens;
    public DbSet<TUserAddress> TUserAddresses { get; set; }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);

        configurationBuilder.Properties<Guid>().HaveConversion<GuidToBytesConverter>();
            
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        aggregator.ApplyAutoConfigurations(modelBuilder, nameRewriter);
        
    }
}

