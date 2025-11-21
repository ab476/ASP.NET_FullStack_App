namespace AuthAPI.Modules.Auth.Models;

public record RefreshTokenPair(string RawToken, string TokenHash);
