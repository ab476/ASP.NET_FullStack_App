using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Common.Extensions;
public static partial class ServiceCollectionExtensions
{
    public static IServiceCollection ConfigureOptionsFromSection<T>(this IServiceCollection services)
            where T : class, new()
    {
        services.AddSingleton(static resolver =>
        {
            string sectionName = typeof(T).Name;
            var configuration = resolver.GetRequiredService<IConfiguration>();
            var options = new T();
            configuration.GetSection(sectionName).Bind(options);
            return Options.Create(options);
        });

        return services;
    }
}
