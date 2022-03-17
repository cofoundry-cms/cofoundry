namespace Cofoundry.Domain;

/// <summary>
/// This message is published when a user is updated, including when a username or
/// email address is updated, which also publishes their own more specific events 
/// <see cref="UserEmailUpdatedMessage"/> and <see cref="UserUsernameUpdatedMessage"/>.
/// </summary>
public class UserUpdatedMessage
{
    /// <summary>
    /// The <see cref="IUserAreaDefinition.UserAreaCode"/> of the user area
    /// the user was added to.
    /// </summary>
    public string UserAreaCode { get; set; }

    /// <summary>
    /// Id of the user that has been updated.
    /// </summary>
    public int UserId { get; set; }
}
