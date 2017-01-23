using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// This message is published when the data directly
    /// associated with a page has been updated. To eb notified
    /// when any part of a page (including versions) changes you
    /// should subscribe to IPageContentUpdatedMessage
    /// </summary>
    public class PageUpdatedMessage : IPageContentUpdatedMessage
    {
        /// <summary>
        /// Id of the page that has been updated
        /// </summary>
        public int PageId { get; set; }

        /// <summary>
        /// True if the page was in a published state when it was updated
        /// </summary>
        public bool HasPublishedVersionChanged { get; set; }
    }
}
