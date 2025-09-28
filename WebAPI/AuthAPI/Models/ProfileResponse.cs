namespace AuthAPI.Models;

public class ProfileResponse
{
    public string Email { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string? LastName { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? PhoneNumber { get; set; }
    public DateTime? LastLoggedIn { get; set; }
    public DateTime? CreatedOn { get; set; }
}
