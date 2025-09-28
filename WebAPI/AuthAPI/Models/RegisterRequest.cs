namespace AuthAPI.Models;

public class RegisterRequest
{
    [Required]
    [StringLength(50, ErrorMessage = "First Name cannot be longer than 50 characters.")]
    public string FirstName { get; set; } = string.Empty;

    [StringLength(50, ErrorMessage = "Last Name cannot be longer than 50 characters.")]
    public string? LastName { get; set; }

    [Required(ErrorMessage = "Email Id is required")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    public string Email { get; set; } = string.Empty;

    [DataType(DataType.Date)]
    public DateTime? DateOfBirth { get; set; }

    [Required(ErrorMessage = "Phone number is required")]
    [Phone(ErrorMessage = "Please enter a valid phone number")]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters.")]
    public string Password { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Password and confirmation password do not match.")]
    public string ConfirmPassword { get; set; } = string.Empty;
}
