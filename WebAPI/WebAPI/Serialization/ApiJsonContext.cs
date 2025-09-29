using System.Text.Json.Serialization;

namespace WebAPI.Serialization;

[JsonSerializable(typeof(ApiResponse<bool>))]
[JsonSerializable(typeof(ApiResponse<string>))]
internal partial class ApiJsonContext : JsonSerializerContext { }



