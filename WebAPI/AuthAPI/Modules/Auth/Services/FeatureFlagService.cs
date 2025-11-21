using AuthAPI.Data.Models;

namespace AuthAPI.Modules.Auth.Services;

public class FeatureFlagService(AuthDbContext db) : IFeatureFlagService
{
    public Task<IEnumerable<string>> GetFeatureFlagsForUserAsync(TUser user)
    {
        // simplest: stored as comma separated JSON in user.FeatureFlagsJson
        if (string.IsNullOrEmpty(user.FeatureFlagsJson)) return Task.FromResult(Enumerable.Empty<string>());
        var arr = System.Text.Json.JsonSerializer.Deserialize<string[]>(user.FeatureFlagsJson) ?? Array.Empty<string>();
        return Task.FromResult(arr.AsEnumerable());
    }
}
