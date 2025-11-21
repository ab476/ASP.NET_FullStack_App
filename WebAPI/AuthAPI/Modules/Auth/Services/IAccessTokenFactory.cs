using System.Security.Claims;

namespace AuthAPI.Modules.Auth.Services;

public interface IAccessTokenFactory
{
    string CreateToken(IEnumerable<Claim> claims);
}