using AuthAPI.Services.UserAddresses.Contracts;

namespace AuthAPI.Services.UserAddresses.Validators;

public class UpdateUserAddressRequestValidator : AbstractValidator<UpdateUserAddressRequest>
{
    public UpdateUserAddressRequestValidator()
    {
        RuleFor(x => x.Street)
            .NotEmpty();

        RuleFor(x => x.City)
            .NotEmpty();

        RuleFor(x => x.State)
            .NotEmpty();

        RuleFor(x => x.PostalCode)
            .NotEmpty();

        RuleFor(x => x.Country)
            .NotEmpty();
    }
}