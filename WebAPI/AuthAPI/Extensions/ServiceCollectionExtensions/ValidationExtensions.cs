using AuthAPI.Models.Validators;

namespace AuthAPI.Extensions.ServiceCollectionExtensions;

public static class ValidationExtensions
{
    public static IServiceCollection AddValidationServices(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<LoginRequestValidator>();
        return services;
    }
}