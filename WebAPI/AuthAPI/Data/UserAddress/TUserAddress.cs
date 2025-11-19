using AuthAPI.Data.User;

namespace AuthAPI.Data.UserAddress;

public class TUserAddress
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public virtual TUser User { get; set; } = null!;
    public required string Street { get; set; }
    public required string City { get; set; }
    public required string State { get; set; }
    public required string PostalCode { get; set; }
    public required string Country { get; set; }

    //Audit Columns
    public DateTime CreatedOn { get; set; }
    public DateTime? ModifiedOn { get; set; }
}