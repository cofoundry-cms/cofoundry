namespace Cofoundry.Domain;

/// <summary>
/// Moves a block up or down within a multi-block region 
/// on a custom entity page.
/// </summary>
public class MoveCustomEntityVersionPageBlockCommand : ICommand, ILoggableCommand
{
    /// <summary>
    /// Database id of the block to mvoe.
    /// </summary>
    [Required]
    [PositiveInteger]
    public int CustomEntityVersionPageBlockId { get; set; }

    /// <summary>
    /// The direction to move the block within the collection.
    /// </summary>
    public OrderedItemMoveDirection Direction { get; set; }
}
