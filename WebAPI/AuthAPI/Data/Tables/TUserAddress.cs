using System.ComponentModel.DataAnnotations.Schema;

namespace AuthAPI.Data.Tables;

public class TUserAddress
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public virtual TUser User { get; set; } = null!;
    public string Street { get; set; } = null!;
    public string City { get; set; } = null!;
    public string State { get; set; } = null!;
    public string PostalCode { get; set; } = null!;
    public string Country { get; set; } = null!;
    public bool IsActive { get; set; }

    //Audit Columns
    public DateTime? CreatedOn { get; set; }
    public DateTime? ModifiedOn { get; set; }
}
