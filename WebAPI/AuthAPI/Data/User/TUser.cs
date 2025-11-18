using AuthAPI.Data.UserAddress;

namespace AuthAPI.Data.User;

public class TUser : IdentityUser<Guid>
{
    public virtual ICollection<TUserAddress> Addresses { get; set; } = [];
}
