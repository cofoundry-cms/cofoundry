using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Message published when a block has been added to a page
    /// </summary>
    public class PageVersionBlockAddedMessage : IPageContentUpdatedMessage
    {
        /// <summary>
        /// Id of the page affected by the change
        /// </summary>
        public int PageId { get; set; }

        /// <summary>
        /// The id of the newly created PageVersionBlock
        /// </summary>
        public int PageVersionBlockId { get; set; }

        /// <summary>
        /// Always false because blocks can only be added to draft versions
        /// </summary>
        public bool HasPublishedVersionChanged { get { return false; } }
    }
}
