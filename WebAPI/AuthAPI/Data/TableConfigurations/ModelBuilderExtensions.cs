using Common.Generators;

namespace AuthAPI.Data.TableConfigurations;

[AutoConfigurable(typeof(TRoleClaim), typeof(TRoleClaimConfiguration))]
[AutoConfigurable(typeof(TUserAddress), typeof(TUserAddressConfiguration))]
public partial class ModelBuilderExtensions
{
}
