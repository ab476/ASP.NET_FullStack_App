namespace Shared.Response;

public record ApiResponse<T>(
    bool Success,
    T? Data = default!,
    string Message = ""
);

