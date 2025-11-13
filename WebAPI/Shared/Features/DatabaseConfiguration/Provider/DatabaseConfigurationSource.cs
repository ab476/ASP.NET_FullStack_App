namespace Common.Features.DatabaseConfiguration.Provider;

internal class DatabaseConfigurationSource(DatabaseConfigurationProvider provider) : IConfigurationSource
{
    private readonly DatabaseConfigurationProvider _provider = provider;

    public IConfigurationProvider Build(IConfigurationBuilder builder) => _provider;
}
