#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Common.Constants;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class ResourceNames
{
    // Containers
    public const string Redis = "redis-cache";
    public const string MySql = "mysql-server";

    // Databases
    public const string ConfigurationDb = "config-db";
    public const string AuthDb = "auth-db";

    // Applications
    public const string AuthApi = "auth-api";
    public const string WebApp = "webapp";

    // Environment Variables
    public const string NextJsPort = "PORT";
}
