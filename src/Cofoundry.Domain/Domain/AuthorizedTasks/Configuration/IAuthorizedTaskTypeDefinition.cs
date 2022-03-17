namespace Cofoundry.Domain;

/// <summary>
/// Defines a category that groups together authoried tasks of
/// the same type e.g. "Password Reset" or "Email Verification".
/// </summary>
public interface IAuthorizedTaskTypeDefinition
{
    /// <summary>
    /// A unique 6 character code that can be used to reference the type. 
    /// The code should contain only single-byte (non-unicode) characters
    /// and although case-insensitive, the convention is to use uppercase
    /// e.g. "COFACR" represents the Cofoundry account recovery task.
    /// </summary>
    string AuthorizedTaskTypeCode { get; }

    /// <summary>
    /// A unique name that succintly describes the task. Max 20 characters.
    /// </summary>
    string Name { get; }
}
