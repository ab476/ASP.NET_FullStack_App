namespace Common.Features.DatabaseConfiguration.Data;
public class ConfigurationDbContext(DbContextOptions<ConfigurationDbContext> options) : DbContext(options)
{
    private readonly INameHelper nameHelper = NameHelper.NameHelper.Default;
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
            .HasDatabaseName(nameHelper.RewriteName("IX_Config_LastUpdated"));

    }
}

