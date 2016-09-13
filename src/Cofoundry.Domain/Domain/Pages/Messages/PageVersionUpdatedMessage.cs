using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class PageVersionUpdatedMessage
    {
        public int PageId { get; set; }

        public int PageVersionId { get; set; }
    }
}
