namespace AuthAPI.Extensions.ServiceCollectionExtensions;

public static class HostExtensions
{
    public static IHostBuilder ConfigureDefaultServiceProvider(
        this IHostBuilder hostBuilder,
        IHostEnvironment env)
    {
        hostBuilder.UseDefaultServiceProvider(options =>
        {
            options.ValidateScopes = env.IsDevelopment();
            options.ValidateOnBuild = true;
        });

        return hostBuilder;
    }
}
