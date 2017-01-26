using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Message published when a draft version of a page has been deleted
    /// </summary>
    public class PageDraftVersionDeletedMessage : IPageContentUpdatedMessage
    {
        /// <summary>
        /// Id of the page that the deleted version was deleted from
        /// </summary>
        public int PageId { get; set; }

        /// <summary>
        /// Id of the version that was deleted
        /// </summary>
        public int PageVersionId { get; set; }

        /// <summary>
        /// Always false because deleting drafts has no effect on the published version
        /// </summary>
        public bool HasPublishedVersionChanged
        {
            get { return false; }
        }
    }
}
