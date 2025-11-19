using AuthAPI.Data.Role;
using AuthAPI.Data.RoleClaim;
using AuthAPI.Data.User;
using AuthAPI.Data.UserAddress;
using AuthAPI.Data.UserClaim;
using AuthAPI.Data.UserLogin;
using AuthAPI.Data.UserRole;
using AuthAPI.Data.UserToken;
using Common.Features.NameHelper;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace AuthAPI.Data;


public class AuthDbContext(DbContextOptions<AuthDbContext> options)
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
    private readonly INameHelper nameHelper = NameHelper.Default;
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

        new TUserConfiguration(nameHelper).Configure(modelBuilder.Entity<TUser>());
        new TRoleConfiguration(nameHelper).Configure(modelBuilder.Entity<TRole>());

        new TUserClaimConfiguration(nameHelper).Configure(modelBuilder.Entity<TUserClaim>());
        new TRoleClaimConfiguration(nameHelper).Configure(modelBuilder.Entity<TRoleClaim>());

        new TUserRoleConfiguration(nameHelper).Configure(modelBuilder.Entity<TUserRole>());
        new TUserLoginConfiguration(nameHelper).Configure(modelBuilder.Entity<TUserLogin>());
        new TUserAddressConfiguration(nameHelper).Configure(modelBuilder.Entity<TUserAddress>());
        new TUserTokenConfiguration(nameHelper).Configure(modelBuilder.Entity<TUserToken>());

    }
}