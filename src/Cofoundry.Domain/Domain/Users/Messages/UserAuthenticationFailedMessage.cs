namespace Cofoundry.Domain;

/// <summary>
/// Triggered when an user fails to authenticate e.g. when
/// logging in or validating a password change request.
/// </summary>
public class UserAuthenticationFailedMessage
{
    /// <summary>
    /// The <see cref="IUserAreaDefinition.UserAreaCode"/> of the user area 
    /// attempting to be logged in to.
    /// </summary>
    public string UserAreaCode { get; set; }

    /// <summary>
    /// The username used in the authentication attempt. This is expected to be in a 
    /// "uniquified" format, as this should have been already processed whenever 
    /// this needs to be called.
    /// </summary>
    public string Username { get; set; }
}
