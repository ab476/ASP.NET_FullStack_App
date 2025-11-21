using AuthAPI.Data.Models;

namespace AuthAPI.Extensions.ServiceCollectionExtensions;

public static class IdentityExtensions
{
    public static IServiceCollection AddIdentityServices(this IServiceCollection services)
    {
        services.AddIdentity<TUser, TRole>()
                .AddEntityFrameworkStores<AuthDbContext>();

        return services;
    }
}