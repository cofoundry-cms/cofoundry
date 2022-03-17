namespace Cofoundry.Domain;

/// <summary>
/// Message published when a user is deleted.
/// </summary>
public class UserDeletedMessage
{
    /// <summary>
    /// The <see cref="IUserAreaDefinition.UserAreaCode"/> of the user area
    /// the user belonged to.
    /// </summary>
    public string UserAreaCode { get; set; }

    /// <summary>
    /// Id of the user that has been deleted.
    /// </summary>
    public int UserId { get; set; }
}
