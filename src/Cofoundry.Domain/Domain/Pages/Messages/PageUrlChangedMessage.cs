using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Message published when properties that affect the
    /// url of a page has changed (e.g. PageDirectoryId, UrlSlug, 
    /// LocaleId etc)
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
    }
}
