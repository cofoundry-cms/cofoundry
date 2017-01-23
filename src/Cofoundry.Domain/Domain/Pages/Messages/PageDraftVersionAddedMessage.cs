using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Message published when a draft version is added to a page
    /// </summary>
    public class PageDraftVersionAddedMessage
    {
        /// <summary>
        /// Id of the page the draft was added to
        /// </summary>
        public int PageId { get; set; }

        /// <summary>
        /// Id of the newly created page version
        /// </summary>
        public int PageVersionId { get; set; }
    }
}
