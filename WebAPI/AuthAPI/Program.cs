using AuthAPI.BackgroundService;
using AuthAPI.Data;
using AuthAPI.Data.Role;
using AuthAPI.Data.TableConfigurations;
using AuthAPI.Data.User;
using AuthAPI.Endpoints;
using AuthAPI.Extensions.ServiceCollectionExtensions;
using AuthAPI.Models.Validators;
using AuthAPI.Services.SqlSchema;
using AuthAPI.Services.Time;
using EFCore.NamingConventions.Internal;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

// Host configuration
builder.Host.ConfigureDefaultServiceProvider(builder.Environment);

// Service registration
services.AddSwaggerDocumentation();
services.AddAppOptions(builder.Configuration);
services.AddInfrastructureServices();
services.AddDatabaseServices(builder.Configuration);
services.AddIdentityServices();
services.AddValidationServices();
services.AddAuthenticationServices(builder.Configuration);
services.AddControllerServices();


var app = builder.Build();
app.UseSwaggerDocumentation(app.Environment);

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapKeyGenEndpoints();

app.Run();

public partial class Program { }

// dotnet ef migrations add InitialCreate --output-dir Migrations/SQLiteMigrations