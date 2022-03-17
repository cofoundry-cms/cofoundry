namespace Cofoundry.Domain;

/// <summary>
/// This message is published when the security stamp for a user
/// is updated which happens when key user credential data changes
/// such as a username, password or account recovery information.
/// The message is not published when a new user is created.
/// </summary>
public class UserSecurityStampUpdatedMessage
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
