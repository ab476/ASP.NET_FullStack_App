namespace AuthAPI.Modules.Auth.DTOs;

public record AuthResponse(string AccessToken, string RefreshToken, int AccessTokenExpiresInSeconds);
