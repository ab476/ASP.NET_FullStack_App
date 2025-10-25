namespace AuthAPI.Features.Users;

// 🔹 Request Models
public record RegisterRequest
{
    // Required properties
    public required string UserName { get; init; }
    public required string Email { get; init; }
    public required string Password { get; init; }

    // Optional properties
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public DateTime? DateOfBirth { get; init; }

    // Navigation property for one-to-many relationship
    public virtual ICollection<UserAddressRequest> Addresses { get; init; } = [];
}
public record UserAddressRequest
{
    // Foreign key to TUser
    public Guid UserId { get; init; }

    // Address fields
    public required string Street { get; init; } 
    public required string City { get; init; } 
    public required string State { get; init; }
    public required string PostalCode { get; init; }
    public required string Country { get; init; } 
}
public record RoleRequest(string RoleName);
public record ResetPasswordRequest(string NewPassword);

// 🔹 Response Models
public record UserResponse(Guid Id, string Username, string Email);

// 🔹 Common Result Wrapper
public class OperationResult<T>
{
    public bool Succeeded { get; protected set; }
    public IEnumerable<string>? Errors { get; protected set; }
    public T? Data { get; protected set; }

    private static OperationResult<T> Ok(T? data = default) =>
        new() { Succeeded = true, Data = data };

    private static OperationResult<T> Fail(IEnumerable<string>? errors) =>
        new() { Succeeded = false, Errors = errors };

    // --- Implicit conversion from IdentityResult ---
    public static implicit operator OperationResult<T>(IdentityResult result)
        => result.Succeeded
            ? Ok(default)
            : Fail(result.Errors.Select(e => e.Description));

    public static implicit operator OperationResult<T>(T data) 
        => Ok(data);

    public static implicit operator OperationResult<T>(string error)
        => Fail([error]);

    public static implicit operator OperationResult<T>(string[] errors)
        => Fail(errors);
}

public class OperationResult : OperationResult<object?>
{
    public static OperationResult Ok() =>
        new() { Succeeded = true, Data = null };

    public static OperationResult Fail(IEnumerable<string>? errors) =>
        new() { Succeeded = false, Errors = errors };

    // --- Implicit conversion from IdentityResult ---
    public static implicit operator OperationResult(IdentityResult result)
        => result.Succeeded
            ? Ok()
            : Fail(result.Errors.Select(e => e.Description));

    public static implicit operator OperationResult(string error)
        => Fail([error]);
}
