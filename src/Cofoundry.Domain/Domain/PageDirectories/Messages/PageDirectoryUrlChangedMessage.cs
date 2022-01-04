namespace Cofoundry.Domain
{
    /// <summary>
    /// Message published when properties that affect the URL 
    /// of a page directory has changed (e.g. ParentPageDirectoryId or UrlSlug).
    /// This message is also triggered when the URL indirectly changes because a parent
    /// directory URL has changed.
    /// </summary>
    public class PageDirectoryUrlChangedMessage
    {
        /// <summary>
        /// Id of the page directory that has the url updated
        /// </summary>
        public int PageDirectoryId { get; set; }

        /// <summary>
        /// The full (relative) URL of the directory before the change, formatted with the leading
        /// slash but excluding the trailing slash e.g. "/parent-directory/child-directory".
        /// </summary
        public string OldFullUrlPath { get; set; }
    }
}
