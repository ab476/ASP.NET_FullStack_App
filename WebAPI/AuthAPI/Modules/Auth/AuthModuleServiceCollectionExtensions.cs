using AuthAPI.Modules.Auth.Infrastructure;
using AuthAPI.Modules.Auth.Middleware;
using AuthAPI.Modules.Auth.Repositories;
using AuthAPI.Modules.Auth.Services;
using AuthAPI.Modules.Auth.Settings;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace AuthAPI.Modules.Auth;

public static class AuthModuleServiceCollectionExtensions
{
    public static IServiceCollection AddAuthModule(this IServiceCollection services, IConfiguration config)
    {
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IFeatureFlagService, FeatureFlagService>();
        services.AddScoped<ITenantResolver, TenantResolver>();

        services.Configure<JwtSettings>(config.GetSection("Jwt"));
        services.Configure<RefreshTokenSettings>(config.GetSection("RefreshToken"));


        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.TryAddSingleton<IRefreshTokenFactory, RefreshTokenFactory>();
        services.TryAddScoped<IRefreshTokenValidator, RefreshTokenValidator>();
        services.TryAddSingleton<IAccessTokenFactory ,AccessTokenFactory>();
        services.AddScoped<TokenService>(); // or ITokenService
        services.AddSingleton<IDomainEventPublisher, NoopDomainEventPublisher>();

        // Identity assumed configured elsewhere
        return services;
    }
}