using AuthAPI.Modules.Auth.Models;
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
    private readonly JwtSecurityTokenHandler _tokenHandler = new();

    public AccessTokenFactory(IOptions<JwtSettings> jwtOptions, ITimeProvider clock)
    {
        _jwt = jwtOptions.Value;
        _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Secret));
        _creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256);
        _clock = clock;

    }

    public string CreateToken(IEnumerable<Claim> customClaims)
    {
        ArgumentNullException.ThrowIfNull(customClaims);

        var now = _clock.UtcNow;
        var expires = now.AddMinutes(_jwt.AccessTokenMinutes);

        // Standard claims added automatically
        var standardClaims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Iat, ToUnixEpoch(now).ToString(), ClaimValueTypes.Integer64),
            new(JwtRegisteredClaimNames.Nbf, ToUnixEpoch(now).ToString(), ClaimValueTypes.Integer64),
            new(JwtRegisteredClaimNames.Exp, ToUnixEpoch(expires).ToString(), ClaimValueTypes.Integer64),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),

            // Frontend convenience claim (many JavaScript apps prefer this)
            new(JwtClaimTypes.ExpUnix, ToUnixEpoch(expires).ToString(), ClaimValueTypes.Integer64)
        };

        var claimIdentity = new ClaimsIdentity(standardClaims.Concat(customClaims));

        var descriptor = new SecurityTokenDescriptor
        {
            Subject = claimIdentity,
            Issuer = _jwt.Issuer,
            Audience = _jwt.Audience,
            SigningCredentials = _creds,
            IssuedAt = now,
            NotBefore = now,
            Expires = expires
        };

        var token = _tokenHandler.CreateToken(descriptor);
        return _tokenHandler.WriteToken(token);
    }

    // Convert DateTime to Unix epoch seconds
    private static long ToUnixEpoch(DateTime utc)
        => new DateTimeOffset(utc).ToUnixTimeSeconds();
}
