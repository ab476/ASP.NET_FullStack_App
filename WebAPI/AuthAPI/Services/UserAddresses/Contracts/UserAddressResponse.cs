namespace AuthAPI.Services.UserAddresses.Contracts;

public class UserAddressResponse
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }

    public required string Street { get; set; }
    public required string City { get; set; }
    public required string State { get; set; }
    public required string PostalCode { get; set; }
    public required string Country { get; set; }

    public DateTime CreatedOn { get; set; }
    public DateTime? ModifiedOn { get; set; }
}