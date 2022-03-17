using Cofoundry.Domain.Internal;

namespace Cofoundry.Domain;

/// <summary>
/// Service for configuring the password policy for a specific user area. To customize
/// the password policy for a specified user area, simply implement this interface for
/// your chosen user area.
/// </summary>
/// <typeparam name="TUserAreaDefinition">
/// The definition type of the user area to configure a custom
/// password policy for.
/// </typeparam>
public interface IPasswordPolicyConfiguration<TUserAreaDefinition> : IPasswordPolicyConfigurationBase
{
}
