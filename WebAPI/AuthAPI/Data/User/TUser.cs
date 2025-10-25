using AuthAPI.Data.UserAddress;

namespace AuthAPI.Data.User;

public class TUser : IdentityUser<Guid>
{
    // Extended properties
    public string? FirstName { get; set; }
    public string? LastName { get; set; }

    public DateTime? DateOfBirth { get; set; }
    public DateTime? LastLogin { get; set; }
    //Audit Columns
    public DateTime? CreatedOn { get; set; } = DateTime.UtcNow;
    public DateTime? ModifiedOn { get; set; }
    // Navigation property for one-to-many relationsip
    public virtual ICollection<TUserAddress> Addresses { get; set; } = [];
}
