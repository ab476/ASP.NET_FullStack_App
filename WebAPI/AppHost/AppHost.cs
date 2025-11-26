


var builder = DistributedApplication.CreateBuilder(args);

// 1. Add the Redis resource
var redisCache = builder.AddRedis(ResourceNames.Redis)
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent);

var mysql = builder.AddMySql(ResourceNames.MySql)
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent)
    .WithPhpMyAdmin();

var configurationDB = mysql.AddDatabase(ResourceNames.ConfigurationDb);
var authDB = mysql.AddDatabase(ResourceNames.AuthDb);

// Add API project
var authApi = builder
    .AddProject<Projects.AuthAPI>("auth-api")
    .WithReference(configurationDB)
    .WithReference(authDB)
    .WithReference(redisCache)
    .WaitFor(redisCache)
    .WaitFor(configurationDB)
    .WaitFor(authDB);

// Next.js app
var next = builder.AddNpmApp("webapp", "../NextJSWebApp", "dev")
                  .WithHttpEndpoint(env: "PORT")
                  .WithReference(authApi)
                  .PublishAsDockerFile(); // Next.js uses PORT env


builder.Build().Run();
