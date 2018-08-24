using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Represents a query which can be paged.
    /// </summary>
    public interface IPageableQuery
    {
        /// <summary>
        /// 1-based number of the page to display i.e. 1 = first page.
        /// </summary>
        int PageNumber { get; set; }

        /// <summary>
        /// The number of items to display on a page. If this is set
        /// to 0 or less then no paging is applied.
        /// </summary>
        int PageSize { get; set; }
    }
}
