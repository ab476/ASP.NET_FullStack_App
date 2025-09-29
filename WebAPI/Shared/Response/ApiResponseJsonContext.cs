using System.Text.Json.Serialization;

namespace Common.Response;

[JsonSerializable(typeof(ApiResponse<object?>))]
internal partial class ApiResponseJsonContext : JsonSerializerContext
{
}
