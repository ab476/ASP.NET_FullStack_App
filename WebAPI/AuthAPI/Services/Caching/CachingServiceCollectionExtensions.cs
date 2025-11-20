using Microsoft.Extensions.DependencyInjection.Extensions;

namespace AuthAPI.Services.Caching;

public static class CachingServiceCollectionExtensions
{
    public static IServiceCollection AddRedisDistributedCache(this IServiceCollection services, IHostApplicationBuilder builder)
    {
        builder.AddRedisDistributedCache("cache");

        var env = builder.Environment;

        //services.AddDistributedMemoryCache();
        services.TryAddSingleton<ICachePrefixProvider>(new CachePrefixProvider(env.ApplicationName, env.EnvironmentName));

        services.TryAddSingleton<ICacheService, CacheService>();

        return services;
    }
}