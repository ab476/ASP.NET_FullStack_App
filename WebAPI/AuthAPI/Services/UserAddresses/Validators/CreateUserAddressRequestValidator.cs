using AuthAPI.Services.UserAddresses.Contracts;
using AuthAPI.Services.Users;

namespace AuthAPI.Services.UserAddresses.Validators;



public class CreateUserAddressRequestValidator : AbstractValidator<CreateUserAddressRequest>
{
    public CreateUserAddressRequestValidator(IUserRepository users)
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .MustAsync(users.ExistsAsync)
            .WithMessage("User does not exist.");

        RuleFor(x => x.Street)
            .NotEmpty().WithMessage("Street is required.");

        RuleFor(x => x.City)
            .NotEmpty().WithMessage("City is required.");

        RuleFor(x => x.State)
            .NotEmpty().WithMessage("State is required.");

        RuleFor(x => x.PostalCode)
            .NotEmpty().WithMessage("PostalCode is required.");

        RuleFor(x => x.Country)
            .NotEmpty().WithMessage("Country is required.");
    }
}