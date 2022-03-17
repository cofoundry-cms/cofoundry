namespace Cofoundry.Domain;

/// <summary>
/// This message is published when a user signs out.
/// </summary>
public class UserSignedOutMessage
{
    /// <summary>
    /// The <see cref="IUserAreaDefinition.UserAreaCode"/> of the user area
    /// the user belongs to.
    /// </summary>
    public string UserAreaCode { get; set; }

    /// <summary>
    /// Id of the user that has signed out. If logout was due to a user deletion,
    /// then the user will not exist.
    /// </summary>
    public int UserId { get; set; }
}
