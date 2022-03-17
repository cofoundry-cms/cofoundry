namespace Cofoundry.Domain.Internal;

/// <summary>
/// Factory for creating and configuring <see cref="IPasswordPolicy"/>
/// for a specific user area, handling the resolution of any custom
/// <see cref="IPasswordPolicyConfiguration{TUserArea}"/> instances. 
/// </summary>
public interface IPasswordPolicyFactory
{
    /// <summary>
    /// Creates an <see cref="IPasswordPolicy"/> configured for
    /// the specific user area.
    /// </summary>
    /// <param name="userAreaCode">
    /// The <see cref="IUserAreaDefinition.UserAreaCode"/> of the user area to get the 
    /// policy for. If the user area does not exist then an exception will be thrown.
    /// </param>
    /// <returns></returns>
    IPasswordPolicy Create(string userAreaCode);
}
