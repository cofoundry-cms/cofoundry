using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Message published when a block has been removed from a page
    /// </summary>
    public class PageVersionBlockDeletedMessage : IPageContentUpdatedMessage
    {
        /// <summary>
        /// Id of the page affected by the change
        /// </summary>
        public int PageId { get; set; }

        /// <summary>
        /// The id of the PageVersion that contained the deleted block
        /// </summary>
        public int PageVersionId { get; set; }

        /// <summary>
        /// Always false because blocks can only be modified on draft versions
        /// </summary>
        public bool HasPublishedVersionChanged { get { return false; } }
    }
}
