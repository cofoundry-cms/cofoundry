namespace Cofoundry.Domain;

/// <summary>
/// Used to decide where to move an OrderedItem
/// in a list
/// </summary>
public enum OrderedItemMoveDirection
{
    /// <summary>
    /// Move up one place in the collection
    /// </summary>
    Up,

    /// <summary>
    /// Move down one place in the collection
    /// </summary>
    Down
}
