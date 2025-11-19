namespace AuthAPI.Data.Role;

public class TRole : IdentityRole<Guid>
{
    // Extended property
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    //Audit Columns
    public DateTime? CreatedOn { get; set; }
    public DateTime? ModifiedOn { get; set; }
}