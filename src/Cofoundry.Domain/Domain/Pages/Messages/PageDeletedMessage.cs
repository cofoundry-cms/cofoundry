using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Message published when a page is deleted
    /// </summary>
    public class PageDeletedMessage
    {
        /// <summary>
        /// Id of the page that has been deleted
        /// </summary>
        public int PageId { get; set; }
    }
}
