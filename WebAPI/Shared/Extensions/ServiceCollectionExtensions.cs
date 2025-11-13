namespace Common.Extensions;


public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Bind a configuration section to a class and register it as a singleton for the specified interface.
    /// </summary>
    /// <typeparam name="TInterface">The interface type to register.</typeparam>
    /// <typeparam name="TImplementation">The concrete class to bind from configuration.</typeparam>
    /// <param name="services">IServiceCollection instance.</param>
    /// <param name="sectionName">Configuration section name.</param>
    /// <returns>IServiceCollection</returns>
    public static IServiceCollection AddOptionsFromConfiguration<TInterface, TImplementation>(
        this IServiceCollection services, string sectionName)
        where TInterface : class
        where TImplementation : class, TInterface, new()
    {
        if (string.IsNullOrWhiteSpace(sectionName))
            throw new ArgumentException("Section name cannot be null or empty.", nameof(sectionName));

        services.AddSingleton<TInterface>(resolver =>
        {
            var configuration = resolver.GetRequiredService<IConfiguration>();
            var options = new TImplementation();
            configuration.GetSection(sectionName).Bind(options);
            return options;
        });

        return services;
    }
}

