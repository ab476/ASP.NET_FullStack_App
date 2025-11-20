namespace AuthAPI.Services.Caching;

public class AuthApiCacheKeys
{
    public static string UserAddressById(Guid id) =>
        $"user_address:id:{id}";

    public static string AddressesByUserId(Guid userId) =>
        $"user_address:user:{userId}";

    public static string UserIdByAddressId(Guid addressId) =>
        $"user_address:user_id_by_address:{addressId}";
}
