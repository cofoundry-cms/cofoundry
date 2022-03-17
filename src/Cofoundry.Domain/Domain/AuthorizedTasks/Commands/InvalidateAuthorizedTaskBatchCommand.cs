namespace Cofoundry.Domain;

/// <summary>
/// Invalidates all incomplete tasks associated with the <see cref="UserId"/>. Invalidations 
/// can optionally be limited to a specific task type or range of task types by specifying 
/// <see cref="AuthorizedTaskTypeCodes"/>.
/// </summary>
public class InvalidateAuthorizedTaskBatchCommand : ICommand, ILoggableCommand
{
    public InvalidateAuthorizedTaskBatchCommand() { }

    /// <summary>
    /// Initializes a new <see cref="InvalidateAuthorizedTaskBatchCommand"/> instance
    /// with command parameters.
    /// </summary>
    /// <param name="userId">Id of the user invalidate task tokens for.</param>
    /// <param name="authorizedTaskTypeCodes">Optionally restrict invalidation to a range of <see cref="IAuthorizedTaskTypeDefinition.AuthorizedTaskTypeCode"/>.</param>
    public InvalidateAuthorizedTaskBatchCommand(int userId, params string[] authorizedTaskTypeCodes)
    {
        UserId = userId;
        AuthorizedTaskTypeCodes = authorizedTaskTypeCodes;
    }

    /// <summary>
    /// Id of the user to invalidate task tokens for.
    /// </summary>
    [Required]
    [PositiveInteger]
    public int UserId { get; set; }

    /// <summary>
    /// Optionally restrict invalidation to a range of <see cref="IAuthorizedTaskTypeDefinition.AuthorizedTaskTypeCode"/>.
    /// </summary>
    public ICollection<string> AuthorizedTaskTypeCodes { get; set; }
}
