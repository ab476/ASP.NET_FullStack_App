using AuthAPI.Data.Configurations;
using AuthAPI.Data.Models;
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

    public DbSet<TRefreshToken> TRefreshTokens { get; set; }
    public DbSet<TSingleUseToken> TSingleUseTokens { get; set; }
    public DbSet<TApiKey> TApiKeys { get; set; }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);

        configurationBuilder.Properties<Guid>().HaveConversion<GuidToBytesConverter>();

    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new TUserConfiguration(nameHelper));
        modelBuilder.ApplyConfiguration(new TRoleConfiguration(nameHelper));
        modelBuilder.ApplyConfiguration(new TUserClaimConfiguration(nameHelper));
        modelBuilder.ApplyConfiguration(new TRoleClaimConfiguration(nameHelper));
        modelBuilder.ApplyConfiguration(new TUserRoleConfiguration(nameHelper));
        modelBuilder.ApplyConfiguration(new TUserLoginConfiguration(nameHelper));
        modelBuilder.ApplyConfiguration(new TUserAddressConfiguration(nameHelper));
        modelBuilder.ApplyConfiguration(new TUserTokenConfiguration(nameHelper));

        // custom tokens
        modelBuilder.ApplyConfiguration(new TRefreshTokenConfiguration());
        modelBuilder.ApplyConfiguration(new TSingleUseTokenConfiguration());
        modelBuilder.ApplyConfiguration(new TApiKeyConfiguration());

    }
}