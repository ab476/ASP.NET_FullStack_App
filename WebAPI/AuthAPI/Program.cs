using AuthAPI.Data;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;    

services.ConfigureOptionsFromSection<DatabaseOptions>();
services.ConfigureOptionsFromSection<EmailSettings>();

builder.Services.AddSingleton<AuthDbContextFactory>();
builder.Services.AddDbContext<AuthDbContext>((sp, options) =>
{
    var factory = sp.GetRequiredService<AuthDbContextFactory>();
    factory.Configure(options); // ✅ delegates provider choice to the factory
});
builder.Services.AddIdentity<TUser, TRole>()
    .AddEntityFrameworkStores<AuthDbContext>();

builder.Services.AddControllers();

var app = builder.Build();

app.MapControllers();

app.Run();

// dotnet ef migrations add InitialCreate --output-dir Migrations/SQLiteMigrations