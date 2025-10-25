namespace AuthAPI.Features.Users;

public static class UserExtensions
{
    public static IServiceCollection AddUserFeature(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        return services;
    }
}
