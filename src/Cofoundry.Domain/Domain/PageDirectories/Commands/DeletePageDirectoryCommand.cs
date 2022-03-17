namespace Cofoundry.Domain;

/// <summary>
/// Removes a page directory from the system. The root directory cannot
/// be deleted.
/// </summary>
public class DeletePageDirectoryCommand : ICommand, ILoggableCommand
{
    public DeletePageDirectoryCommand() { }

    /// <summary>
    /// Initialized the command.
    /// </summary>
    /// <param name="pageDirectoryId">Database id of the page directory to delete</param>
    public DeletePageDirectoryCommand(int pageDirectoryId)
    {
        PageDirectoryId = pageDirectoryId;
    }

    /// <summary>
    /// Database id of the page directory to delete
    /// </summary>
    [Required]
    [PositiveInteger]
    public int PageDirectoryId { get; set; }
}
