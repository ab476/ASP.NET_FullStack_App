using Microsoft.Extensions.DependencyInjection.Extensions;

namespace AuthAPI.Services.Time;

public static class TimeProviderServiceCollectionExtensions
{
    public static IServiceCollection AddTimeProvider(this IServiceCollection services)
    {
        services.TryAddSingleton<ITimeProvider, SystemTimeProvider>();
        return services;
    }
}
