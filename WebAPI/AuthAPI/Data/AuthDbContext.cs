using EFCore.NamingConventions.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Options;
using System.Globalization;

namespace AuthAPI.Data;


public class AuthDbContext(DbContextOptions<AuthDbContext> options, IOptions<DatabaseOptions> dbConfig)
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

    private readonly DatabaseOptions _DBConfig = dbConfig.Value;
    private DatabaseType ActiveDatabase => _DBConfig.ActiveDatabase;
    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);

        configurationBuilder.Properties<Guid>().HaveConversion<GuidToBytesConverter>();
            
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        var nameRewriter = ActiveDatabase is DatabaseType.Oracle 
            ? new UpperSnakeCaseNameRewriter(CultureInfo.CurrentCulture)
            : new SnakeCaseNameRewriter(CultureInfo.CurrentCulture);

        var extensions = new TableConfigurations.EntityConfigurationAggregator();
        extensions.ApplyAutoConfigurations(modelBuilder, nameRewriter);
        
    }
}

