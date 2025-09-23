using Microsoft.EntityFrameworkCore;

namespace AuthAPI.Data;


public class AuthDbContext(DbContextOptions<AuthDbContext> options) : IdentityDbContext<TUser, TRole, Guid>(options)
{
}
