using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace Common.Response;

/// <summary>
/// A wrapper around <see cref="ActionResult"/> that automatically
/// serializes responses into a standardized <see cref="ApiResponse{T}"/> format.
/// </summary>
/// <typeparam name="T">The type of the response data.</typeparam>
public sealed class ApiActionResult<T> : IConvertToActionResult
{
    /// <summary>
    /// Gets the raw <see cref="ActionResult"/> if the response
    /// was created from an existing MVC result (e.g. <c>NotFound()</c>, <c>Ok()</c>).
    /// </summary>
    public ActionResult? Result { get; }

    /// <summary>
    /// Gets the strongly typed API response if the result was created
    /// from a <typeparamref name="T"/> value or <see cref="ApiResponse{T}"/>.
    /// </summary>
    public ApiResponse<T>? Response { get; }

    #region Constructors

    /// <summary>
    /// Initializes a new instance of <see cref="ApiActionResult{T}"/> 
    /// wrapping a raw value into an <see cref="ApiResponse{T}"/>.
    /// </summary>
    /// <param name="value">The value of the response.</param>
    /// <param name="message">Optional message associated with the response.</param>
    /// <param name="success">Indicates whether the response represents success.</param>
    public ApiActionResult(T value, string message = "", bool success = true)
    {
        Response = new ApiResponse<T>(success, value, message);
    }

    /// <summary>
    /// Initializes a new instance of <see cref="ApiActionResult{T}"/> 
    /// from an existing <see cref="ApiResponse{T}"/>.
    /// </summary>
    /// <param name="response">The API response object.</param>
    public ApiActionResult(ApiResponse<T> response)
    {
        Response = response;
    }

    /// <summary>
    /// Initializes a new instance of <see cref="ApiActionResult{T}"/> 
    /// from an existing <see cref="ActionResult"/>.
    /// </summary>
    /// <param name="result">The MVC action result (e.g. <c>NotFound()</c>).</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="result"/> is null.</exception>
    public ApiActionResult(ActionResult result)
    {
        Result = result ?? throw new ArgumentNullException(nameof(result));
    }

    #endregion

    #region Implicit conversions

    /// <summary>
    /// Implicitly converts a raw value into an <see cref="ApiActionResult{T}"/>
    /// with <c>Success=true</c>.
    /// </summary>
    public static implicit operator ApiActionResult<T>(T value)
    {
        return new ApiActionResult<T>(value);
    }

    /// <summary>
    /// Implicitly converts an <see cref="ApiResponse{T}"/> into an <see cref="ApiActionResult{T}"/>.
    /// </summary>
    public static implicit operator ApiActionResult<T>(ApiResponse<T> response)
    {
        return new ApiActionResult<T>(response);
    }

    /// <summary>
    /// Implicitly converts an <see cref="ActionResult"/> into an <see cref="ApiActionResult{T}"/>.
    /// Useful for returning <c>NotFound()</c>, <c>BadRequest()</c>, etc.
    /// </summary>
    public static implicit operator ApiActionResult<T>(ActionResult result)
    {
        return new ApiActionResult<T>(result);
    }

    /// <summary>
    /// Implicitly converts a tuple <c>(string message, T? value)</c> into 
    /// an <see cref="ApiActionResult{T}"/> with <c>Success=false</c>.
    /// </summary>
    public static implicit operator ApiActionResult<T>((string Messsge, T? value) tuple)
    {
        return new ApiActionResult<T>(tuple.value!, tuple.Messsge, false);
    }

    /// <summary>
    /// Implicitly converts a tuple <c>(T value, string message)</c> into 
    /// an <see cref="ApiActionResult{T}"/> with <c>Success=true</c>.
    /// </summary>
    public static implicit operator ApiActionResult<T>((T value, string Messsge) tuple)
    {
        return new ApiActionResult<T>(tuple.value, tuple.Messsge, true);
    }

    #endregion

    /// <summary>
    /// Converts this wrapper into a framework-compatible <see cref="IActionResult"/>.
    /// </summary>
    IActionResult IConvertToActionResult.Convert()
    {
        if (Result != null)
            return Result;

        return new JsonResult(Response)
        {
            StatusCode = Response?.Success == true ? (int)HttpStatusCode.OK : null
        };
    }
}