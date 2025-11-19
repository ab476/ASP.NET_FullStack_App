namespace AuthAPI.Extensions;

public static class HostEnvironmentExtensions
{
    /// <summary>
    /// Checks whether the application is running with the custom test flag.
    /// </summary>
    public static bool IsTestEnvironment(this IConfiguration config)
    {
        return config.GetValue<bool>("TEST_ENV") == true;
    }
}