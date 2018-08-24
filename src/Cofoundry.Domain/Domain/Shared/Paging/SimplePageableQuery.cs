using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// A simple query model that contains only paging settings. This can 
    /// be used a base class for more complex pageable queries.
    /// </summary>
    public class SimplePageableQuery : IPageableQuery
    {
        /// <summary>
        /// 1-based number of the page to display i.e. 1 = first page.
        /// </summary>
        public int PageNumber { get; set; }

        /// <summary>
        /// The number of items to display on a page. If this is set
        /// to 0 or less then no paging is applied.
        /// </summary>
        public int PageSize { get; set; }
    }
}
