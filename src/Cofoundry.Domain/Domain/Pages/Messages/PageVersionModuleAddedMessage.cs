using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class PageVersionModuleAddedMessage : IPageContentUpdatedMessage
    {
        public int PageId { get; set; }

        public int PageVersionModuleId { get; set; }

        public bool HasPublishedVersionChanged { get; set; }
    }
}
