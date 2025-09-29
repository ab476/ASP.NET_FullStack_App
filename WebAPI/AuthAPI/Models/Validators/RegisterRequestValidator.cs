namespace AuthAPI.Models.Validators;


public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First Name is required")
            .MaximumLength(50).WithMessage("First Name cannot be longer than 50 characters.");

        RuleFor(x => x.LastName)
            .MaximumLength(50).WithMessage("Last Name cannot be longer than 50 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.LastName));

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email Id is required")
            .EmailAddress().WithMessage("Invalid email address");

        RuleFor(x => x.DateOfBirth)
            .LessThan(DateTime.Today).WithMessage("Date of Birth must be in the past")
            .When(x => x.DateOfBirth.HasValue);

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required")
            .Matches(@"^\+?\d{10,15}$").WithMessage("Please enter a valid phone number");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters.")
            .MaximumLength(100).WithMessage("Password cannot exceed 100 characters.");

        RuleFor(x => x.ConfirmPassword)
            .Equal(x => x.Password).WithMessage("Password and confirmation password do not match");
    }
}
