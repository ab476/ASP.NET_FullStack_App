using Microsoft.EntityFrameworkCore;

namespace Common.Features.DatabaseConfiguration.Data;

public class ConfigurationDbContext(DbContextOptions<ConfigurationDbContext> options) : DbContext(options)
{
    public DbSet<ConfigurationEntry> Configurations => Set<ConfigurationEntry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new ConfigurationEntryConfiguration());
    }
}
