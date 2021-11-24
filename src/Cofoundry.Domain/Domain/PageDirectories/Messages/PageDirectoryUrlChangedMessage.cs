using Microsoft.AspNetCore.Http;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Message published when properties that affect the url 
    /// of a page directory has changed (e.g. ParentPageDirectoryId or UrlSlug).
    /// This message is also triggered when the url indirectly changes because a parent
    /// directory url has changed.
    /// </summary>
    public class PageDirectoryUrlChangedMessage
    {
        /// <summary>
        /// Id of the page directory that has the url updated
        /// </summary>
        public int PageDirectoryId { get; set; }

        /// <summary>
        /// The full (relative) url of the directory before the change, formatted with the leading
        /// slash but excluding the trailing slash e.g. "/parent-directory/child-directory".
        /// </summary
        public PathString OldFullUrlPath { get; set; }
    }
}
