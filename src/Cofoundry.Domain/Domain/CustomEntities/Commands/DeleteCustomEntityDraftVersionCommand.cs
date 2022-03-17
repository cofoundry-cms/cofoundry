namespace Cofoundry.Domain;

/// <summary>
/// Deletes the draft verison of a custom entity permanently if 
/// it exists. If no draft exists then no action is taken.
/// </summary>
public class DeleteCustomEntityDraftVersionCommand : ICommand, ILoggableCommand
{
    /// <summary>
    /// Id of the custom entity to delete the draft version for.
    /// </summary>
    [PositiveInteger]
    [Required]
    public int CustomEntityId { get; set; }
}
