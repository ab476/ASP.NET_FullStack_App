using Common.Generators;

namespace AuthAPI.Data.TableConfigurations;
[ApplyEntityConfiguration(typeof(TUser), typeof(TUserConfiguration))]
[ApplyEntityConfiguration(typeof(TRole), typeof(TRoleConfiguration))]
[ApplyEntityConfiguration(typeof(TUserClaim), typeof(TUserClaimConfiguration))]
[ApplyEntityConfiguration(typeof(TUserRole), typeof(TUserRoleConfiguration))]
[ApplyEntityConfiguration(typeof(TUserLogin), typeof(TUserLoginConfiguration))]
[ApplyEntityConfiguration(typeof(TRoleClaim), typeof(TRoleClaimConfiguration))]
[ApplyEntityConfiguration(typeof(TUserToken), typeof(TUserTokenConfiguration))]
[ApplyEntityConfiguration(typeof(TUserAddress), typeof(TUserAddressConfiguration))]
public partial class EntityConfigurationAggregator
{
}
