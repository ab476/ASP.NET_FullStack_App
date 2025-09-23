using AuthAPI.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<AuthDbContextFactory>();
builder.Services.AddDbContext<AuthDbContext>((sp, options) =>
{
    var factory = sp.GetRequiredService<AuthDbContextFactory>();
    factory.Configure(options); // ✅ delegates provider choice to the factory
});
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<AuthDbContext>();
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();
