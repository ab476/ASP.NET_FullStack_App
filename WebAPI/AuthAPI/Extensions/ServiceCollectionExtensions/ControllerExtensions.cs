namespace AuthAPI.Extensions.ServiceCollectionExtensions;

public static class ControllerExtensions
{
    public static IServiceCollection AddControllerServices(this IServiceCollection services)
    {
        services.AddControllers();
        return services;
    }
}