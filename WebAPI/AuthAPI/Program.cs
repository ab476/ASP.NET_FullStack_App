using AuthAPI.BackgroundService;
using AuthAPI.Data;
using AuthAPI.Data.Role;
using AuthAPI.Data.TableConfigurations;
using AuthAPI.Data.User;
using AuthAPI.Endpoints;
using AuthAPI.Models.Validators;
using AuthAPI.Services.SqlSchema;
using AuthAPI.Services.Time;
using Common.Swagger;
using EFCore.NamingConventions.Internal;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;    
builder.Host.UseDefaultServiceProvider(options =>
{
    options.ValidateScopes = builder.Environment.IsDevelopment();
    options.ValidateOnBuild = true;
});
services.AddEndpointsApiExplorer();
services.AddSwaggerGen(options =>
{
    options.OperationAsyncFilter<ApiActionResultOperationFilter>();
});
services.AddHttpClient();
services.AddOptionsFromConfiguration<IMultiDatabaseConfig, MultiDatabaseConfig>("MultiDatabaseConfig");
services.AddOptionsFromConfiguration<IEmailSettings, EmailSettings>("EmailSettings");
builder.Services.AddSingleton<INameRewriter>(provider =>
{
    var culture = CultureInfo.CurrentCulture;
    var dbConfig = provider.GetRequiredService<IMultiDatabaseConfig>();
    return dbConfig.ActiveDatabase switch
    {
        DatabaseType.Oracle => new UpperSnakeCaseNameRewriter(culture),
        _ => new SnakeCaseNameRewriter(culture),
    };
});
builder.Services.AddSingleton<IEntityConfigurationAggregator, EntityConfigurationAggregator>();
builder.Services.AddSingleton<AuthDbContextFactory>();
builder.Services.AddDbContextPool<AuthDbContext>((sp, options) =>
{
    var factory = sp.GetRequiredService<AuthDbContextFactory>();
    factory.Configure(options); // ✅ delegates provider choice to the factory
});
builder.Services.AddIdentity<TUser, TRole>()
    .AddEntityFrameworkStores<AuthDbContext>();

builder.Services.AddValidatorsFromAssemblyContaining<LoginRequestValidator>();

builder.Services.AddControllers();

services.AddTimeProvider()
    .AddSchemaService();

// Add Authentication & JWT
services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var jwt = builder.Configuration.GetSection("Jwt");
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwt["Issuer"],
        ValidAudience = jwt["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!))
    };
});

builder.Services.AddHostedService<InitializeDatabaseService>();

var app = builder.Build();
var env = app.Environment;
if (env.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "AuthAPI v1");
        c.RoutePrefix = string.Empty; // UI available at root in dev
    });
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapKeyGenEndpoints();

app.Run();

// dotnet ef migrations add InitialCreate --output-dir Migrations/SQLiteMigrations