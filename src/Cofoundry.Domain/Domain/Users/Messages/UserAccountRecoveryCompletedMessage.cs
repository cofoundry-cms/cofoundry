namespace Cofoundry.Domain;

/// <summary>
/// This message is published when a user successfully completes the account 
/// recovery process via <see cref="CompleteUserAccountRecoveryViaEmailCommand"/>. This
/// command will also trigger <see cref="UserPasswordUpdatedMessage"/>.
/// </summary>
public class UserAccountRecoveryCompletedMessage
{
    /// <summary>
    /// The <see cref="IUserAreaDefinition.UserAreaCode"/> of the user area
    /// the user belongs to.
    /// </summary>
    public string UserAreaCode { get; set; }

    /// <summary>
    /// Id of the user recovering their account.
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// Unique identifier for the underlying authorized task that was used to manage the reset request.
    /// </summary>
    public Guid AuthorizedTaskId { get; set; }
}
