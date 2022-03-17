namespace Cofoundry.Domain.Internal;

/// <summary>
/// Service for working with password policies.
/// </summary>
public interface IPasswordPolicyService
{
    /// <summary>
    /// Gets the password policy description for a user area.
    /// </summary>
    /// <param name="userAreaCode">
    /// The <see cref="IUserAreaDefinition.UserAreaCode"/> of the user area to get the 
    /// policy for. If the user area does not exist then an exception will be thrown.
    /// </param>
    PasswordPolicyDescription GetDescription(string userAreaCode);

    /// <summary>
    /// Validates a new password conforms to the configured policy, throwing a 
    /// <see cref="ValidationErrorException"/> if any errors are found.
    /// </summary>
    /// <param name="context">
    /// A context object that contains the new password and other data about the request that may
    /// be useful for validating the password.
    /// </param>
    Task ValidateAsync(INewPasswordValidationContext context);
}
