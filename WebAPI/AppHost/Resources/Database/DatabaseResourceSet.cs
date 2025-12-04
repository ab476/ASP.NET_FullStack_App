namespace AppHost.Resources.Database;

public sealed record DatabaseResourceSet(
    IResourceBuilder<IResourceWithConnectionString> Instance,
    IResourceBuilder<IResourceWithConnectionString> ConfigurationDb,
    IResourceBuilder<IResourceWithConnectionString> AuthDb
);