namespace Cofoundry.Domain;

/// <summary>
/// Marks an authorized task as complete. The command does not validate 
/// the authorization task or token, which is expected to be done prior 
/// to invoking this command. To validate an auhtorized task token use
/// <see cref="ValidateAuthorizedTaskTokenQuery"/>.
/// </summary>
public class CompleteAuthorizedTaskCommand : ICommand, ILoggableCommand
{
    public CompleteAuthorizedTaskCommand()
    {
    }

    /// <summary>
    /// Initializes a new <see cref="CompleteAuthorizedTaskCommand"/> instance
    /// with command parameters.
    /// </summary>
    /// <param name="authorizedTaskId">Unique identifier for the task to mark as complete.</param>
    public CompleteAuthorizedTaskCommand(Guid authorizedTaskId)
    {
        AuthorizedTaskId = authorizedTaskId;
    }

    /// <summary>
    /// Unique identifier for the task to mark as complete.
    /// </summary>
    [Required]
    public Guid AuthorizedTaskId { get; set; }
}
