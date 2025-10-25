using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Extensions;
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddOptionsFromConfiguration<T>(this IServiceCollection services)
        where T : class, new()
    {
        return services.AddOptionsFromConfiguration<T>(typeof(T).Name);
    }

    public static IServiceCollection AddOptionsFromConfiguration<T>(this IServiceCollection services, string sectionName)
        where T : class, new()
    {
        if (string.IsNullOrWhiteSpace(sectionName))
            throw new ArgumentException("Section name cannot be null or empty.", nameof(sectionName));

        services.AddSingleton(resolver =>
        {
            var configuration = resolver.GetRequiredService<IConfiguration>();
            var options = new T();
            configuration.GetSection(sectionName).Bind(options);
            return options;
        });

        return services;
    }
}
