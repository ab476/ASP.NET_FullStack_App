using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace Common.Response;

public sealed class ApiActionResult<T> : IConvertToActionResult
{
    public ApiResponse<T>? Response { get; }
    public IActionResult? ActionResult { get; }

    public ApiActionResult(T value, string message = "", bool success = true, HttpStatusCode? statusCode = null)
    {
        Response = new ApiResponse<T>(success, value, message, statusCode);
    }

    public ApiActionResult(ApiResponse<T> response)
    {
        Response = response ?? throw new ArgumentNullException(nameof(response));
    }

    public ApiActionResult(IActionResult actionResult)
    {
        ActionResult = actionResult ?? throw new ArgumentNullException(nameof(actionResult));
    }

    public static implicit operator ApiActionResult<T>(T value) => new(value);
    public static implicit operator ApiActionResult<T>(ApiResponse<T> response) => new(response);
    public static implicit operator ApiActionResult<T>((string message, T? value) tuple) => new(tuple.value!, tuple.message, false);
    public static implicit operator ApiActionResult<T>((T value, string message) tuple) => new(tuple.value, tuple.message, true);
    public static implicit operator ApiActionResult<T>(ActionResult result) => new(result);

    IActionResult IConvertToActionResult.Convert()
    {
        if (ActionResult != null)
            return ActionResult;

        var status = Response?.StatusCode
                     ?? (Response?.Success == true ? HttpStatusCode.OK : HttpStatusCode.BadRequest);

        return new ObjectResult(Response) { StatusCode = (int)status };
    }
}
