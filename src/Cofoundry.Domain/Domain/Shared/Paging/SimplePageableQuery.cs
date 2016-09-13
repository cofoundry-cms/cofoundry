using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class SimplePageableQuery : IPageableQuery
    {
        /// <summary>
        /// 1-based number of the page to display (1 = first page)/
        /// </summary>
        public int PageNumber { get; set; }

        /// <summary>
        /// The number of items to display on a page
        /// </summary>
        public int PageSize { get; set; }
    }
}
