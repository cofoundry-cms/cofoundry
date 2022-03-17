namespace Cofoundry.Domain;

/// <summary>
/// Sets the status of a page to un-published, but does not
/// remove the publish date, which is preserved so that it
/// can be used as a default when the user chooses to publish
/// again.
/// </summary>
public class UnPublishPageCommand : ICommand, ILoggableCommand
{
    /// <summary>
    /// Sets the status of a page to un-published, but does not
    /// remove the publish date, which is preserved so that it
    /// can be used as a default when the user chooses to publish
    /// again.
    /// </summary>
    public UnPublishPageCommand()
    {
    }

    /// <summary>
    /// Sets the status of a page to un-published, but does not
    /// remove the publish date, which is preserved so that it
    /// can be used as a default when the user chooses to publish
    /// again.
    /// </summary>
    /// <param name="pageId">The database id of the page to unpublish.</param>
    public UnPublishPageCommand(int pageId)
    {
        PageId = pageId;
    }

    /// <summary>
    /// The id of the page to set un-published.
    /// </summary>
    [PositiveInteger]
    [Required]
    public int PageId { get; set; }
}
