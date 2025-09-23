using System.Text.Json.Serialization;

namespace Shared.Response;

[JsonSerializable(typeof(ApiResponse<object?>))]
internal partial class ApiResponseJsonContext : JsonSerializerContext
{
}
