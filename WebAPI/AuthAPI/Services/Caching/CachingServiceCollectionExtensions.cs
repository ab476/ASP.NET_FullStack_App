using Microsoft.Extensions.DependencyInjection.Extensions;

namespace AuthAPI.Services.Caching;

public static class CachingServiceCollectionExtensions
{
    public static IServiceCollection AddRedisDistributedCache(this IServiceCollection services, IHostApplicationBuilder builder)
    {
        builder.AddRedisDistributedCache("cache");

        //services.AddDistributedMemoryCache();

        services.TryAddSingleton(typeof(ICacheService<>), typeof(CacheService<>)); // open generic registration

        services.TryAddSingleton<ICacheServiceFactory, CacheServiceFactory>();

        return services;
    }
}