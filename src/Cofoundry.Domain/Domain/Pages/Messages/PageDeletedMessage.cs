namespace Cofoundry.Domain;

/// <summary>
/// Message published when a page is deleted. When a directory is
/// deleted, all child directories and pages are also deleted which will
/// generate multiple messages in the same batch, one for each page 
/// deleted.
/// </summary>
public class PageDeletedMessage
{
    /// <summary>
    /// Id of the page that has been deleted.
    /// </summary>
    public int PageId { get; set; }

    /// <summary>
    /// The full (relative) url of the deleted page with the leading
    /// slash, but excluding the trailing slash e.g. "/my-directory/example-page".
    /// </summary
    public string FullUrlPath { get; set; }
}
