


using AppHost.Resources.Database;
using Microsoft.Extensions.Configuration;

var builder = DistributedApplication.CreateBuilder(args);
// 1. Add the Redis resource
var redisCache = builder.AddRedis(ResourceNames.Redis)
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent);

// 2. database Server

var (dbOptions, _, configurationDb, authDb) = builder.ConfigureDatabaseContainers();
// Add API project
var authApi = builder
    .AddProject<Projects.AuthAPI>("auth-api")
    .WithEnvironment($"{DatabaseOptions.Database}__{nameof(DatabaseOptions.Provider)}", dbOptions.Provider.ToString())
    .WithReference(configurationDb)
    .WithReference(authDb)
    .WithReference(redisCache)
    .WaitFor(redisCache)
    .WaitFor(configurationDb)
    .WaitFor(authDb);

// Next.js app
var next = builder.AddNpmApp("next-app", "../NextJSWebApp", "dev")
                  .WithHttpEndpoint(env: "PORT")
                  .WithReference(authApi)
                  .PublishAsDockerFile();
                  //.WithOtlpExporter(OtlpProtocol.HttpProtobuf); // Next.js uses PORT env


builder.Build().Run();
