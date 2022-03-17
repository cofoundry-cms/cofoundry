namespace Cofoundry.Domain;

/// <summary>
/// Writes a log entry to the FailedAuthenticationAttempt table indicating
/// an unsuccessful login attempt occurred.
/// </summary>
public class LogFailedAuthenticationAttemptCommand : ICommand
{
    public LogFailedAuthenticationAttemptCommand() { }

    /// <param name="userAreaCode">
    /// The <see cref="IUserAreaDefinition.UserAreaCode"/> of the user area 
    /// attempting to be authenticated.
    /// </param>
    /// <param name="username">
    /// The username used in the authentication attempt. This is expected to be in a 
    /// "uniquified" format, as this should have been already processed whenever 
    /// this needs to be called.
    /// </param>
    public LogFailedAuthenticationAttemptCommand(string userAreaCode, string username)
    {
        Username = username;
        UserAreaCode = userAreaCode;
    }

    /// <summary>
    /// The <see cref="IUserAreaDefinition.UserAreaCode"/> of the user area 
    /// attempting to be authenticated.
    /// </summary>
    [Required]
    public string UserAreaCode { get; set; }

    /// <summary>
    /// The username used in the authentication attempt. This is expected to be in a 
    /// "uniquified" format, as this should have been already processed whenever 
    /// this needs to be called.
    /// </summary>
    [Required]
    public string Username { get; set; }
}
