var builder = DistributedApplication.CreateBuilder(args);

// 1. Add the Redis resource
var redisCache = builder.AddRedis("cache")
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent);

var configurationDB = builder.AddMySql("configuration-db")
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent)
    .AddDatabase("ConfigurationDatabase")
    ;
// Add API project
var authApi = builder
    .AddProject<Projects.AuthAPI>("auth-api")
    .WithReference(configurationDB)
    .WithReference(redisCache)
    .WaitFor(redisCache)
    .WaitFor(configurationDB);

// Next.js app
var next = builder.AddNpmApp("webapp", "../NextJSWebApp", "dev")
                  .WithHttpEndpoint(env: "PORT")
                  .PublishAsDockerFile(); // Next.js uses PORT env
builder.Build().Run();
