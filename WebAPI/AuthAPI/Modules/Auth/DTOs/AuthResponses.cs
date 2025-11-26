namespace AuthAPI.Modules.Auth.DTOs;

public record LoginResponse(string AccessToken, string RefreshToken, int AccessTokenExpiresInSeconds);
