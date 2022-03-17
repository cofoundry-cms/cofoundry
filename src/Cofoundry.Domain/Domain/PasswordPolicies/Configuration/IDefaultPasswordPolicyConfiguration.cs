using Cofoundry.Domain.Internal;

namespace Cofoundry.Domain;

/// <summary>
/// Service for configuring the password policy used by default for all 
/// user areas. You can override the default implementation of this interface 
/// to change the password policy for all user areas. To configure the password
/// policy for a specific user area, implement the generic 
/// <see cref="IPasswordPolicyConfiguration{TUserArea}"/> interface instead.
/// </summary>
public interface IDefaultPasswordPolicyConfiguration : IPasswordPolicyConfigurationBase
{
}
