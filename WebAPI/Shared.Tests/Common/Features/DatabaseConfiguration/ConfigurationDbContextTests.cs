using Common.Features.DatabaseConfiguration.Data;
using EFCore.NamingConventions.Internal;
using Microsoft.EntityFrameworkCore;
using Testcontainers.MySql;

namespace Shared.Tests.Common.Features.DatabaseConfiguration;

public class ConfigurationDbContextTests : IAsyncLifetime
{
    private readonly MySqlContainer _container;

    public ConfigurationDbContextTests()
    {
        _container = new MySqlBuilder()
            .WithImage("mysql:8.0")
            .WithDatabase("testdb")
            .WithUsername("root")
            .WithPassword("password")
            .Build();
    }

    public Task InitializeAsync() => _container.StartAsync();
    public async Task DisposeAsync() => await _container.DisposeAsync();

    private ConfigurationDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ConfigurationDbContext>()
            .UseMySql(_container.GetConnectionString(), ServerVersion.AutoDetect(_container.GetConnectionString()))
            .Options;

        return new ConfigurationDbContext(options, new FakeRewriter());
    }

    private class FakeRewriter : INameRewriter
    {
        public string RewriteName(string name) => name;
    }

    [Fact]
    public void Should_Have_DbSet_Mapped()
    {
        using var ctx = CreateContext();

        ctx.TConfigurations.Should().NotBeNull();
    }

    [Fact]
    public void Should_Set_NoTracking_By_Default()
    {
        using var ctx = CreateContext();

        ctx.ChangeTracker.QueryTrackingBehavior.Should().Be(QueryTrackingBehavior.NoTracking);
    }

    [Fact]
    public void Should_Have_PrimaryKey_On_Key()
    {
        using var ctx = CreateContext();

        var entity = ctx.Model.FindEntityType(typeof(TConfigurationEntry));
        var pk = entity.FindPrimaryKey();

        pk.Properties.Should().ContainSingle()
            .Which.Name.Should().Be("Key");
    }

    [Fact]
    public void Should_Enforce_Key_MaxLength_256()
    {
        using var ctx = CreateContext();

        var prop = ctx.Model.FindEntityType(typeof(TConfigurationEntry)).FindProperty("Key");

        prop.GetMaxLength().Should().Be(256);
        prop.IsNullable.Should().BeFalse();
    }

    [Fact]
    public void Should_Enforce_Value_MaxLength_500()
    {
        using var ctx = CreateContext();

        var prop = ctx.Model.FindEntityType(typeof(TConfigurationEntry)).FindProperty("Value");

        prop.GetMaxLength().Should().Be(500);
        prop.IsNullable.Should().BeFalse();
    }

    [Fact]
    public void Should_Have_Index_On_LastUpdated()
    {
        using var ctx = CreateContext();

        var entity = ctx.Model.FindEntityType(typeof(TConfigurationEntry));

        var index = entity.GetIndexes()
            .FirstOrDefault(i => i.Properties.Any(p => p.Name == "LastUpdated"));

        index.Should().NotBeNull("because the LastUpdated property should have an index defined.");
    }

    [Fact]
    public void Should_Use_Rewriter_For_Index_Name()
    {
        using var ctx = CreateContext();

        var entity = ctx.Model.FindEntityType(typeof(TConfigurationEntry));

        var index = entity.GetIndexes()
            .First(i => i.Properties.Any(p => p.Name == "LastUpdated"));

        index.GetDatabaseName().Should().Be("snake_IX_Config_LastUpdated");
    }
}