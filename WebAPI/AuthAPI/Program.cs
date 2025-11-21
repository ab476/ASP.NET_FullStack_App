using AuthAPI.Endpoints;
using AuthAPI.Extensions.ServiceCollectionExtensions;
using AuthAPI.Features.UserAddresses;
using AuthAPI.Services.Caching;
using AuthAPI.Services.UserAddresses.Endpoints;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;
var configuration = builder.Configuration;


builder.AddServiceDefaults();
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

app.MapDefaultEndpoints();

await app.RunAsync();

public partial class Program { }

// dotnet ef migrations add InitialCreate --output-dir Migrations/SQLiteMigrations