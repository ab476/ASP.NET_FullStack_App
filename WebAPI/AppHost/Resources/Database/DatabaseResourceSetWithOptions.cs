namespace AppHost.Resources.Database;

public sealed record DatabaseResourceSetWithOptions(
    DatabaseOptions Options,
    IResourceBuilder<IResourceWithConnectionString> Instance,
    IResourceBuilder<IResourceWithConnectionString> ConfigurationDb,
    IResourceBuilder<IResourceWithConnectionString> AuthDb
);