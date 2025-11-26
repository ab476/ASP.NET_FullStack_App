using AuthAPI.Helper;
using AuthAPI.Modules.Auth.Models;
using AuthAPI.Modules.Auth.Services;
using AuthAPI.Modules.Auth.Settings;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace Shared.Tests.AuthAPITests.ServiceTests;

public class AccessTokenFactoryTests
{
    private readonly FakeTimeProvider _clock = new();

    private AccessTokenFactory CreateFactory(int minutes = 30)
    {
        var settings = new JwtSettings
        {
            Issuer = "test-issuer",
            Audience = "test-audience",
            Secret = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
            AccessTokenMinutes = minutes
        };

        return new AccessTokenFactory(
            Options.Create(settings),
            _clock
        );
    }

    private static JwtSecurityToken Read(string token)
        => new JwtSecurityTokenHandler().ReadJwtToken(token);


    // ---------------- Test: Standard Claims ----------------

    [Fact]
    public void CreateToken_IncludesStandardClaims()
    {
        // Arrange
        var factory = CreateFactory(minutes: 15);
        var now = _clock.UtcNow;
        var expectedExp = now.AddMinutes(15);

        var customClaims = new[] { new Claim(ClaimTypes.Role, UserRoles.Admin) };

        // Act
        var tokenString = factory.CreateToken(customClaims);
        var jwt = Read(tokenString);

        // Assert using actual token fields
        jwt.ValidFrom.Should().BeCloseTo(now, TimeSpan.FromSeconds(1));
        jwt.ValidTo.Should().BeCloseTo(expectedExp, TimeSpan.FromSeconds(1));

        string Claim(string type) =>
            jwt.Claims.Single(c => c.Type == type).Value;

        // ---- Validate standard claims with 1-second tolerance ----

        FromUnix(Claim(JwtRegisteredClaimNames.Iat))
            .Should().BeCloseTo(now, TimeSpan.FromSeconds(1));

        FromUnix(Claim(JwtRegisteredClaimNames.Nbf))
            .Should().BeCloseTo(now, TimeSpan.FromSeconds(1));

        FromUnix(Claim(JwtRegisteredClaimNames.Exp))
            .Should().BeCloseTo(expectedExp, TimeSpan.FromSeconds(1));

        FromUnix(Claim(JwtClaimTypes.ExpUnix))
            .Should().BeCloseTo(expectedExp, TimeSpan.FromSeconds(1));

        // JTI claim still appears
        jwt.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.Jti);

        // Custom claim still appears
        jwt.Claims.Should().ContainSingle(c => c.Type == ClaimTypes.Role && c.Value == UserRoles.Admin);
    }



    // ---------------- Test: exp_unix claim ----------------

    [Fact]
    public void CreateToken_SetsExpUnixClaim()
    {
        // Arrange
        var factory = CreateFactory(minutes: 60);
        var now = _clock.UtcNow;
        var expectedExp = now.AddMinutes(60);

        // Act
        var token = factory.CreateToken([]);
        var jwt = Read(token);

        // Assert
        jwt.Claims.Should().ContainSingle(c =>
             c.Type == JwtClaimTypes.ExpUnix &&
             c.Value == ToUnix(expectedExp));
    }


    // ---------------- Test: No custom claims allowed null ----------------

    [Fact]
    public void CreateToken_Throws_WhenCustomClaimsNull()
    {
        var factory = CreateFactory();

        Action act = () => factory.CreateToken(null!);

        act.Should().Throw<ArgumentNullException>();
    }


    // ---------------- Helpers ----------------

    private static string ToUnix(DateTime utc)
        => new DateTimeOffset(utc).ToUnixTimeSeconds().ToString();

    private static DateTime FromUnix(string value)
        => DateTimeOffset.FromUnixTimeSeconds(long.Parse(value)).UtcDateTime;
}
