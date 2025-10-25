using AuthAPI.BackgroundService;
using AuthAPI.Data;
using AuthAPI.Data.Role;
using AuthAPI.Data.TableConfigurations;
using AuthAPI.Data.User;
using AuthAPI.Models.Validators;
using AuthAPI.Services.Time;
using Common.Swagger;
using EFCore.NamingConventions.Internal;
using System.Globalization;

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
    var dbConfig = provider.GetRequiredService<MultiDatabaseConfig>();
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

services.AddTimeProvider();

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


app.MapControllers();

app.Run();

// dotnet ef migrations add InitialCreate --output-dir Migrations/SQLiteMigrations