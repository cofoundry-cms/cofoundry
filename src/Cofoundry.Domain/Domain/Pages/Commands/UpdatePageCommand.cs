namespace Cofoundry.Domain;

/// <summary>
/// Updates page properties that aren't specific to a
/// version.
/// </summary>
public class UpdatePageCommand : IPatchableByIdCommand, ILoggableCommand
{
    /// <summary>
    /// Database id of the page to update.
    /// </summary>
    [PositiveInteger]
    [Required]
    public int PageId { get; set; }

    /// <summary>
    /// Tags can be used to categorize an entity.
    /// </summary>
    public ICollection<string> Tags { get; set; }
}
