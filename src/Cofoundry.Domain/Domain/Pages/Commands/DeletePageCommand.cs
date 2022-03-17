namespace Cofoundry.Domain;

/// <summary>
/// Deletes a page and all associated versions permanently.
/// </summary>
public class DeletePageCommand : ICommand, ILoggableCommand
{
    /// <summary>
    /// Database id of the page to delete.
    /// </summary>
    [PositiveInteger]
    [Required]
    public int PageId { get; set; }
}
