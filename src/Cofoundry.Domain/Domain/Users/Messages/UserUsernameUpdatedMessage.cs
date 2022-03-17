namespace Cofoundry.Domain;

/// <summary>
/// This message is published when the username for a user is updated. The less
/// specific <see cref="UserUpdatedMessage"/> is also published when a username is updated.
/// </summary>
public class UserUsernameUpdatedMessage
{
    /// <summary>
    /// The <see cref="IUserAreaDefinition.UserAreaCode"/> of the user area
    /// the user belongs to.
    /// </summary>
    public string UserAreaCode { get; set; }

    /// <summary>
    /// Id of the user that has been updated.
    /// </summary>
    public int UserId { get; set; }
}
