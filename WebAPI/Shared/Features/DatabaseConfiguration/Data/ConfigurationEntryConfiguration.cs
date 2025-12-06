using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Common.Features.DatabaseConfiguration.Data;

public class ConfigurationEntryConfiguration : IEntityTypeConfiguration<ConfigurationEntry>
{
    public void Configure(EntityTypeBuilder<ConfigurationEntry> b)
    {
        b.ToTable("configuration_entries");

        // PK
        b.HasKey(e => e.Key);

        b.Property(e => e.Key)
            .IsRequired()
            .HasMaxLength(256);

        // Generic large text (provider will pick appropriate type)
        b.Property(e => e.Value)
            .IsRequired();

        // Timestamp - do NOT use provider-specific SQL
        b.Property(e => e.LastUpdated)
            .IsRequired()
            .ValueGeneratedOnAddOrUpdate();

        // Optimistic concurrency (portable)
        b.Property(e => e.RowVersion)
            .IsConcurrencyToken();

        // Index
        b.HasIndex(e => e.LastUpdated)
            .HasDatabaseName("ix_configuration_last_updated");
    }
}