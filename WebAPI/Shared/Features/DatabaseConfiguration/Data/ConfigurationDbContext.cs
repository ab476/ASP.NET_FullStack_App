using EFCore.NamingConventions.Internal;

namespace Common.Features.DatabaseConfiguration.Data;
public class ConfigurationDbContext(DbContextOptions<ConfigurationDbContext> options, INameRewriter rewriter) : DbContext(options)
{
    public DbSet<TConfigurationEntry> TConfigurations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<TConfigurationEntry>(b =>
        {
            b.HasKey(e => e.Key);

            b.Property(e => e.Key)
                .IsRequired()
                .HasMaxLength(256);

            b.Property(e => e.Value)
                .IsRequired()
                .HasMaxLength(500);

            b.Property(e => e.LastUpdated)
                .IsRequired();
        });

        modelBuilder.Entity<TConfigurationEntry>()
            .HasIndex(e => e.LastUpdated)
            .HasDatabaseName(rewriter.RewriteName("IX_Config_LastUpdated"));

    }
}

