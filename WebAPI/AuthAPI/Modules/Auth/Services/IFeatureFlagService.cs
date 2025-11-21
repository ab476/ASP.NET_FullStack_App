using AuthAPI.Data.Models;

namespace AuthAPI.Modules.Auth.Services;

public interface IFeatureFlagService 
{ 
    Task<IEnumerable<string>> GetFeatureFlagsForUserAsync(TUser user); 
}
