namespace Cofoundry.Domain;

/// <summary>
/// The result data for a successful <see cref="ValidateAuthorizedTaskTokenQuery"/>, which 
/// contains additional data to help with executing the task.
/// </summary>
public class AuthorizedTaskTokenValidationResultData
{
    /// <summary>
    /// Unique identifier for the task.
    /// </summary>
    public Guid AuthorizedTaskId { get; set; }

    /// <summary>
    /// The id of the user the token belongs to.
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// Unique 3 character code identifying this user area the user belongs to.
    /// </summary>
    public string UserAreaCode { get; set; }

    /// <summary>
    /// Data that was optionally included with the task. This might be data used to validate the
    /// task or data to be used once the task is authorized. E.g. when verifying
    /// an email address you could store the email being verified, this can later be used to 
    /// validate that the users email is the same as when the task token was generated.
    /// If you only wanted to change an email address after it has been verified, then you could
    /// store the email in <see cref="TaskData"/>, and then once verified the user can be updated 
    /// with the new email address.
    /// </summary>
    public string TaskData { get; set; }
}
