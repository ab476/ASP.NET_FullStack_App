using AuthAPI.Endpoints;
using AuthAPI.Extensions.ServiceCollectionExtensions;
using AuthAPI.Features.UserAddresses;
using AuthAPI.Services.Caching;
using AuthAPI.Services.UserAddresses.Endpoints;
using Microsoft.Extensions.Caching.Distributed;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;
var configuration = builder.Configuration;



// Host configuration
builder.Host.ConfigureDefaultServiceProvider(builder.Environment);

configuration
    .AddDatabaseConfiguration(services, options =>
    {
        options.ConfigureDbContext = dbOptions =>
        {
            var connectionString = configuration.GetConnectionString("ConfigurationDatabase");
            dbOptions.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
            dbOptions.UseSnakeCaseNamingConvention(CultureInfo.CurrentCulture);
        };
    });

// Service registration
services.AddSwaggerDocumentation()
    .AddAppOptions(configuration)
    .AddInfrastructureServices()
    .AddDatabaseServices(configuration)
    .AddIdentityServices()
    .AddValidationServices()
    .AddAuthenticationServices(configuration)
    .AddControllerServices()
    .AddRedisDistributedCache(builder);

services.AddUserAddressFeature();

var app = builder.Build();
app.UseSwaggerDocumentation(app.Environment);

app.UseExceptionHandler("/error");          // Custom fallback endpoint
app.UseHsts();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app
    .MapUserAddressEndpoints()
    .MapGlobalErrorEndpoint()
    .MapKeyGenEndpoints()
    .MapDatabaseConfigurationEndpoints();
app.MapGet("/data/{key}", async (string key, IDistributedCache cache) =>
{
    // Try to get data from the cache
    var cachedData = await cache.GetAsync(key);

    if (cachedData != null)
    {
        // Data found in cache
        var dataString = Encoding.UTF8.GetString(cachedData);
        return Results.Ok($"From Cache: {dataString}");
    }
    else
    {
        // Data not found, fetch/create it
        var newData = $"New data for {key} at {DateTime.UtcNow}";

        // Convert to byte array
        var dataToCache = Encoding.UTF8.GetBytes(newData);

        // Set cache options (e.g., 5-minute absolute expiration)
        var options = new DistributedCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromMinutes(5));

        // Store data in the cache
        await cache.SetAsync(key, dataToCache, options);

        return Results.Ok($"From Source (cached): {newData}");
    }
})
.WithName("GetData")
.WithOpenApi();
await app.RunAsync();

public partial class Program { }

// dotnet ef migrations add InitialCreate --output-dir Migrations/SQLiteMigrations