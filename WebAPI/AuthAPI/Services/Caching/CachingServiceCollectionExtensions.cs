using Microsoft.Extensions.DependencyInjection.Extensions;

namespace AuthAPI.Services.Caching;

public static class CachingServiceCollectionExtensions
{
    public static IServiceCollection AddCaching(this IServiceCollection services)
    {
        services.AddDistributedMemoryCache();

        services.TryAddSingleton(typeof(ITypedDistributedCache<>), typeof(TypedDistributedCache<>)); // open generic registration

        services.TryAddSingleton<ITypedDistributedCacheFactory, TypedDistributedCacheFactory>();

        return services;
    }
}