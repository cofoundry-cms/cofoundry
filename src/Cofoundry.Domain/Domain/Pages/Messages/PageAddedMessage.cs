using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class PageAddedMessage : IPageContentUpdatedMessage
    {
        public int PageId { get; set; }
        
        public bool HasPublishedVersionChanged { get; set; }
    }
}
