using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Message published when a page is added
    /// </summary>
    public class PageAddedMessage : IPageContentUpdatedMessage
    {
        /// <summary>
        /// Id of the page that was added
        /// </summary>
        public int PageId { get; set; }
        
        /// <summary>
        /// True if the page was published when it was added; otherwise false
        /// </summary>
        public bool HasPublishedVersionChanged { get; set; }
    }
}
