var builder = DistributedApplication.CreateBuilder(args);

var configurationDB = builder.AddMySql("configuration-db")
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent)
   .AddDatabase("ConfigurationDatabase")
    ;
// Add API project
var authApi = builder
    .AddProject<Projects.AuthAPI>("auth-api")
    .WithReference(configurationDB)
    .WaitFor(configurationDB);


builder.Build().Run();
