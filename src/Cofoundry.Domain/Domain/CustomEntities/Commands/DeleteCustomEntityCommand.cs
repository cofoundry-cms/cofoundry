namespace Cofoundry.Domain;

/// <summary>
/// Deletes a custom entity and all associated versions permanently.
/// </summary>
public class DeleteCustomEntityCommand : ICommand, ILoggableCommand
{
    /// <summary>
    /// Database id of the custom entity to delete.
    /// </summary>
    [PositiveInteger]
    [Required]
    public int CustomEntityId { get; set; }
}
