

namespace AuthAPI.Models;

public class ResendConfirmationEmailRequest
{
    [Required(ErrorMessage = "Email Id is required")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    public string Email { get; set; } = string.Empty;
}