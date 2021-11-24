using Microsoft.AspNetCore.Http;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Message published when properties that affect the url 
    /// of a page has changed (e.g. PageDirectoryId, UrlSlug, 
    /// LocaleId etc). This message is not triggered if the url
    /// indirectly changes e.g. if a parent directory changes.
    /// </summary>
    public class PageUrlChangedMessage : IPageContentUpdatedMessage
    {
        /// <summary>
        /// Id of the page that has the url updated
        /// </summary>
        public int PageId { get; set; }

        /// <summary>
        /// True if the page was in a published state when it was updated
        /// </summary>
        public bool HasPublishedVersionChanged { get; set; }

        /// <summary>
        /// The original full (relative) url of the page before the change was made.
        /// The url is formatted with the leading slash, but excluding the trailing 
        /// slash e.g. "/my-directory/example-page".
        /// </summary
        public PathString OldFullUrlPath { get; set; }
    }
}
