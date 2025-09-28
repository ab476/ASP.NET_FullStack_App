using Riok.Mapperly.Abstractions;

namespace AuthAPI.Helper;

[Mapper]
public static partial class MapperExtensions
{
    public static partial TUser MapToTUser(this RegisterRequest registerRequest);
    public static partial ProfileResponse MapToProfileResponse(this TUser user);

}
