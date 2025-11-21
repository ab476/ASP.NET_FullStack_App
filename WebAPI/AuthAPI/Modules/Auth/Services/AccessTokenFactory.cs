using AuthAPI.Modules.Auth.Settings;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace AuthAPI.Modules.Auth.Services;

public class AccessTokenFactory : IAccessTokenFactory
{
    private readonly JwtSettings _jwt;
    private readonly SymmetricSecurityKey _key;
    private readonly SigningCredentials _creds;
    private readonly ITimeProvider _clock;

    public AccessTokenFactory(IOptions<JwtSettings> jwtOptions, ITimeProvider clock)
    {
        _jwt = jwtOptions.Value;
        _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Secret));
        _creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256);
        _clock = clock;
    }

    public string CreateToken(IEnumerable<Claim> claims)
    {
        var now = _clock.UtcNow;
        var descriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = now.AddMinutes(_jwt.AccessTokenMinutes),
            Issuer = _jwt.Issuer,
            Audience = _jwt.Audience,
            SigningCredentials = _creds,
            NotBefore = now
        };

        var handler = new JwtSecurityTokenHandler();
        var token = handler.CreateToken(descriptor);
        return handler.WriteToken(token);
    }
}
