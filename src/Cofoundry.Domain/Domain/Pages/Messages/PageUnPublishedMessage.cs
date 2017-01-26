using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Message published when a page has gone from published to 
    /// draft state.
    /// </summary>
    public class PageUnPublishedMessage : IPageContentUpdatedMessage
    {
        /// <summary>
        /// Id of the page that has been unpublished
        /// </summary>
        public int PageId { get; set; }

        /// <summary>
        /// True, because the published version has been changed to draft
        /// </summary>
        public bool HasPublishedVersionChanged { get { return true; } }
    }
}
