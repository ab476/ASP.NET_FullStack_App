using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Common.Swagger;


public class ApiActionResultOperationFilter : IOperationAsyncFilter
{
    private const string OK = "200";
    private const string BadRequest = "400";
    private const string NotFound = "404";

    public async Task ApplyAsync(OpenApiOperation operation, OperationFilterContext context, CancellationToken cancellationToken)
    {
        var returnType = context.MethodInfo.ReturnType;

        if (!returnType.IsGenericType || returnType.GetGenericTypeDefinition() != typeof(ApiActionResult<>))
            return;

        var typeArg = returnType.GetGenericArguments()[0];

        var response = typeArg.IsAssignableTo(typeof(FileResult))
            ? new OpenApiResponse
            {
                Description = "File download",
                Content = { ["application/octet-stream"] = new OpenApiMediaType { Schema = new OpenApiSchema { Type = "string", Format = "binary" } } }
            }
            : new OpenApiResponse
            {
                Description = "Success",
                Content = { ["application/json"] = new OpenApiMediaType { Schema = await GenerateSchemaAsync(typeArg, context, cancellationToken) } }
            };

        operation.Responses.TryAdd(OK, response);
        operation.Responses.TryAdd(BadRequest, new OpenApiResponse { Description = "Bad Request" });
        operation.Responses.TryAdd(NotFound, new OpenApiResponse { Description = "Not Found" });
    }

    private static Task<OpenApiSchema> GenerateSchemaAsync(Type typeArg, OperationFilterContext context, CancellationToken cancellationToken)
        => Task.FromResult(context.SchemaGenerator.GenerateSchema(typeof(ApiResponse<>).MakeGenericType(typeArg), context.SchemaRepository));
}
