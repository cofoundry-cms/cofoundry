namespace Cofoundry.Domain;

/// <summary>
/// <para>
/// An <see cref="IPasswordPolicy"/> is the result of an <see cref="IPasswordPolicyBuilder"/> and
/// represents a dynamically generated password policy that can be used to validate a new
/// user password. The password policy can be customized for a specific user area by implementing
/// <see cref="IPasswordPolicyConfiguration{TUserArea}"/>.
/// </para>
/// <para>
/// An <see cref="IPasswordPolicy"/> is designed to be short-lived, as it
/// typically includes a collection of password validators that are dependent on the 
/// current DI scope.
/// </para>
/// </summary>
public interface IPasswordPolicy
{
    /// <summary>
    /// A brief description of the policy highlighting the
    /// main criteria e.g. "Passwords must be between 10 and 300 characters.".
    /// This description is designed to be displayed to users to help them choose
    /// their new password.
    /// </summary>
    string Description { get; }

    /// <summary>
    /// A collection of HTML attributes that describe the policy e.g. "minlength", "maxlength"
    /// or "passwordrules".
    /// </summary>
    IDictionary<string, string> Attributes { get; }

    /// <summary>
    /// Returns a full list of password requirements extracted from each of
    /// the validators in the new password policy e.g. "Must be at least 10 characters.", 
    /// "Must be at less than 300 characters.". A developer may choose to list the requirements 
    /// in full to help a user choose their new password, however the list can include a tedious 
    /// list of edge cases such as "Password must not be the same as your current password.".
    /// </summary>
    IEnumerable<string> GetCriteria();

    /// <summary>
    /// Validates a new password conforms to the policy, returning any errors. Note that during 
    /// the validation process all sync validators will be evaluated and can return errors, but
    /// in order to improve performance async validators will only be evaluated if no other errors 
    /// are found.
    /// </summary>
    /// <param name="context">
    /// A context object that contains the new password and other data about the request that may
    /// be useful for validating the password.
    /// </param>
    /// <returns>
    /// Returns any validation errors triggered by a password that violates the policy. If no errors 
    /// are found then an empty collection is returned.
    /// </returns>
    Task<ICollection<ValidationError>> ValidateAsync(INewPasswordValidationContext context);
}
