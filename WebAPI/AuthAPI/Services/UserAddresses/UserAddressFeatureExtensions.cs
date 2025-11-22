using AuthAPI.Services.UserAddresses.Contracts;
using AuthAPI.Services.UserAddresses.Repository;
using AuthAPI.Services.UserAddresses.Service;
using AuthAPI.Services.UserAddresses.Validators;
using AuthAPI.Services.Users;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace AuthAPI.Services.UserAddresses;

public static class UserAddressFeatureExtensions
{
    public static IServiceCollection AddUserAddressFeature(this IServiceCollection services)
    {
        // Repository
        services.TryAddScoped<IUserAddressRepository, UserAddressRepository>();

        // Service
        services.TryAddScoped<IUserAddressService, UserAddressService>();

        // Validators
        services.TryAddScoped<IValidator<CreateUserAddressRequest>, CreateUserAddressRequestValidator>();
        services.TryAddScoped<IValidator<UpdateUserAddressRequest>, UpdateUserAddressRequestValidator>();

        // User Repository (for validating UserId exists)
        services.TryAddScoped<IUserRepository, UserRepository>();

        return services;
    }
}
