namespace AuthAPI.Extensions.ServiceCollectionExtensions;

public static class SwaggerExtensions
{
    public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.OperationAsyncFilter<ApiActionResultOperationFilter>();
        });

        return services;
    }

    public static void UseSwaggerDocumentation(this WebApplication app, IHostEnvironment env)
    {
        if (!env.IsDevelopment())
            return;

        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "AuthAPI v1");
            c.RoutePrefix = string.Empty;
        });
    }
}