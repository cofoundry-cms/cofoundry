namespace Cofoundry.Domain;

/// <summary>
/// Used to validate a username when adding or updating a user.
/// </summary>
public interface IUsernameValidator
{
    /// <summary>
    /// Validates a username, returning any errors found. By default the validator checks that 
    /// the format contains only the characters permitted by the <see cref="UsernameOptions"/> 
    /// configuration settings, as well as checking for uniquness.
    /// </summary>
    Task<IReadOnlyCollection<ValidationError>> GetErrorsAsync(IUsernameValidationContext context);
}
