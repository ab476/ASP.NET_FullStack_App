using AuthAPI.Data.Models;
using AuthAPI.Services.UserAddresses.Contracts;
using Riok.Mapperly.Abstractions;

namespace AuthAPI.Services.UserAddresses;

[Mapper]
public static partial class UserAddressMapper
{
    // ----------- ENTITY → RESPONSE -----------
    [MapperIgnoreSource(nameof(TUserAddress.User))]
    public static partial UserAddressResponse ToResponse(this TUserAddress entity);

    // ----------- CREATE REQUEST → ENTITY -----------
    [MapperIgnoreTarget(nameof(TUserAddress.Id))]
    [MapperIgnoreTarget(nameof(TUserAddress.User))]
    [MapperIgnoreTarget(nameof(TUserAddress.CreatedOn))]
    [MapperIgnoreTarget(nameof(TUserAddress.ModifiedOn))]
    public static partial TUserAddress ToEntity(this CreateUserAddressRequest request);
}