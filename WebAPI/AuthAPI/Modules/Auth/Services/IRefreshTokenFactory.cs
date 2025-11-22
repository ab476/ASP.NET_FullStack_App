// /Modules/Auth/Services/RefreshTokenFactory.cs
using AuthAPI.Data.Models;
using AuthAPI.Modules.Auth.Models;

namespace AuthAPI.Modules.Auth.Services;

public interface IRefreshTokenFactory
{
    string ComputeHash(string rawToken);
    TRefreshToken CreateEntity(Guid userId, string tokenHash, string? deviceId, string? fingerprint);
    RefreshTokenPair CreatePair();
}